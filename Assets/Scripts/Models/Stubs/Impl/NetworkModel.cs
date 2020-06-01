using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using RR.Facilitators.Photon;
using RR.Facilitators.Photon.Impl;
using RR.Models.GamestateModel;
using RR.Models.ShuttersModel;
using RR.Models.DBModel;
using RR.Models.ArenaModel;
using RR.Models.LoginUIModel;
using RR.Models.ModalUIModel;
using RR.Models.NetPlayerModel;
using RR.Utility;
using RR.Utility.Gameplay;

namespace RR.Models.NetworkModel.Impl
{
    public class NetworkModel : Model, INetworkModel
    {
        const int ROOM_MAX_PLAYERS = 4;
        const bool ROOM_IS_OPEN = true;
        const bool ROOM_IS_VISIBLE = true;
        const int ROOM_EMPTY_TTL = 500;
        const int ROOM_PLAYER_TTL = 10000;
        const bool ROOM_PLAYER_PUBLISH_ID = true;
        const string PLAYER_PREFAB_NAME = "NetPlayer";
        const int DEFAULT_BYTE_GROUP = 0;
        const int POST_READY_WAIT = 0;
        const float GAME_DURATION = 60;
        const float READY_CHECK_TIME = 30f;

        public event Action<NetEvent> NetworkEvent = delegate { };
        public event Action<RoomEvent, string> NetRoomEvent = delegate { };
        public event Action<PlayerDetails[]> RoomFilled = delegate { };
        public event Action PlayerSpawned = delegate { };
        public event Action PlayerCameraTransformUpdated = delegate { };

        public int PlayerCount { get { return PhotonNetwork.room != null ? PhotonNetwork.room.PlayerCount : 0; }}
        public PlayerDetails[] CurrentPlayers { get; private set; }
        public int LocalPlayerID { get; private set; }
        public int SessionSeed { get; private set; }
        public string CurrentErrorMessage { get; private set; }

        private string _lastRoom
        {
            get
            {
                return PlayerPrefs.GetString($"{_currentUser.name}-last-room", string.Empty);
            }
            set
            {
                PlayerPrefs.SetString($"{_currentUser.name}-last-room", value);
            }
        }

        public PlayerStatus LocalPlayerStatus { get; private set; }

        private const string ROOM_NAME = "rr-room_";
        private TypedLobby _lobby = new TypedLobby("rr-lobby-default", LobbyType.Default);
        private List<RoomInfo> _roomList = new List<RoomInfo>();
        private IPhotonRelay _photonRelay;
        private CoroutineOwner _co;

        private User _currentUser;
        private Gamestate _gamestate;
        private HashSet<string> _roomBlacklist;
        private Pedestals _pedestalData;
        private DateTime _connectionAttemptStartTime;
        private bool _readyStateOK;
        private bool _playerWasReady;
        private Coroutine _readyCheckCoroutine;
        private Coroutine _timerStartCoroutine;

        public void Initialize()
        {
            PhotonNetwork.sendRate = 60;
            PhotonNetwork.sendRateOnSerialize = 50;

            Models.Get<IGamestateModel>().StateChanged += OnGamestateChanged;
            Models.Get<IArenaModel>().PedestalsConfigured += OnPedestalsConfigured;

            _photonRelay = GameObject.FindObjectOfType<PhotonRelay>() as IPhotonRelay;
            _photonRelay.ConnectedToMaster += OnConnectedToMaster;
            _photonRelay.JoinedLobby += OnJoinedLobby;
            _photonRelay.CreatedRoom += OnCreatedRoom;
            _photonRelay.JoinedRoom += OnJoinedRoom;
            _photonRelay.ReceivedRoomListUpdate += OnReceivedRoomListUpdate;
            _photonRelay.JoinRoomFailed += OnJoinRoomFailed;
            _photonRelay.PlayerConnected += OnPlayerConnected;
            _photonRelay.PlayerDisconnected += OnPlayerDisconnected;
            _photonRelay.ReadyCheckFinished += OnReadyCheckFinished;
            _photonRelay.Disconnected += OnDisconnectedFromPhoton;
            _photonRelay.LeftRoom += OnLeftRoom;
            _photonRelay.GameFinished += OnGameFinished;
            _photonRelay.PlayerDied += OnPlayerDied;
            
            _roomBlacklist = new HashSet<string>();

            var _coroutineOwnerGameObject = new GameObject("NetworkModel_CoroutineOwner");
            _coroutineOwnerGameObject.AddComponent<CoroutineOwner>();
            _co = _coroutineOwnerGameObject.GetComponent<CoroutineOwner>();
        }

        public void SetUserAccount(User user)
        {
            _currentUser = user;   
        }

        public void ConfirmUserReady()
        {
            _playerWasReady = true;
            _photonRelay.PlayerReady();
        }

        private void Connect(string userID)
        {
            PhotonNetwork.playerName = userID;
            _connectionAttemptStartTime = DateTime.Now;
            PhotonNetwork.ConnectUsingSettings("4.2");
        }

        private string CreateRoom()
        {
            var name = $"{ROOM_NAME}{Guid.NewGuid()}";
            var roomOptions = new RoomOptions()
            {
                MaxPlayers = ROOM_MAX_PLAYERS,
                IsOpen = ROOM_IS_OPEN,
                IsVisible = ROOM_IS_VISIBLE,
                EmptyRoomTtl = ROOM_EMPTY_TTL,
                PlayerTtl = ROOM_PLAYER_TTL,
                PublishUserId = ROOM_PLAYER_PUBLISH_ID,
            };

            PhotonNetwork.CreateRoom(name, roomOptions, _lobby);

            return name;
        }

        private void JoinOrCreateRoom()
        {
            foreach (var room in _roomList)
            {
                if (room.PlayerCount < room.MaxPlayers)
                {
                    if (!_roomBlacklist.Contains(room.Name))
                    {
                        Debug.Log($"Found a room [{room.Name}], joining...");
                        _lastRoom = room.Name;
                        PhotonNetwork.JoinRoom(_lastRoom);
                        return;
                    }
                }
            }

            var msg = "";
            if (_roomList.Count == 0) msg = $"No rooms found. Creating room...";
            else msg = $"Rooms found, but no spaces available. Creating room...";

            Debug.Log(msg);
            _lastRoom = CreateRoom();
        }

        private void SpawnPlayer()
        {
            var pedestalData =  _pedestalData[LocalPlayerID];
            var player = PhotonNetwork.Instantiate(PLAYER_PREFAB_NAME, pedestalData.Transform.position, pedestalData.Facing, DEFAULT_BYTE_GROUP);
            player.transform.SetParent(GameObject.FindObjectOfType<PlayerContainer>().transform);
            PhotonNetwork.player.SetScore(100);

            EventDispatcher.Broadcast(PlayerSpawned);
        }

        private void OnGamestateChanged(Gamestate gs)
        {  
            if (_gamestate != gs)
            {
                _gamestate = gs;

                if (_gamestate == Gamestate.Matchmaking)
                {
                    Connect(_currentUser.name);
                }
                else if (_gamestate == Gamestate.Postgame)
                {
                    Models.Get<INetPlayerModel>().PreDisconnectCleanup();
                }
            }
        }

        private IEnumerator PostConnectToMaster(TimeSpan minimumTimeElapsed)
        {
            var diff = DateTime.Now -_connectionAttemptStartTime;
            if (diff < minimumTimeElapsed)
            {
                var wait = minimumTimeElapsed - diff;

                yield return new WaitForSeconds(wait.Seconds);
            }

            EventDispatcher.Broadcast(NetworkEvent, NetEvent.ConnectedToMaster);

            if (_lastRoom != string.Empty)
            {
                Debug.Log($"Room [{_lastRoom}] already cached, attempting to rejoin...");
                PhotonNetwork.ReJoinRoom(_lastRoom);
            }
            else
            {
                PhotonNetwork.JoinLobby(_lobby);
            }
        }

        private IEnumerator PlayerReadyCountdown(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            _photonRelay.ResetReadyCheck();

            if (!_readyStateOK)
            {
                PhotonNetwork.room.IsOpen = PhotonNetwork.room.IsVisible = true;
                Models.Get<IGamestateModel>().SetState(Gamestate.ReadyCheckFailed);
                
                if (!_playerWasReady)
                {
                    CurrentErrorMessage = "You were returned to the lobby because you failed the ready check.";
                    _lastRoom = string.Empty;
                    PhotonNetwork.LeaveRoom(false);
                }
            }

            _readyStateOK = false;
            _playerWasReady = false;
        }

        private void OnRoomFilled()
        {
            PhotonNetwork.room.IsOpen = PhotonNetwork.room.IsVisible = false;

            Models.Get<IGamestateModel>().SetState(Gamestate.ReadyCheck);

            var players = new Dictionary<int, string>();
            foreach (var player in PhotonNetwork.playerList)
            {
                Debug.Log($"NickName: {player.NickName} | ID: {player.ID} | UserId: {player.UserId}");
                players.Add(player.ID, player.NickName);
            }

            var sortedPlayers = from pair in players orderby pair.Key ascending select pair;
            
            PlayerDetails[] playerDetailsArray = new PlayerDetails[sortedPlayers.Count()];
            for (int i = 0; i < sortedPlayers.Count(); i++)
            {
                var details = new PlayerDetails(sortedPlayers.ElementAt(i).Key, i, sortedPlayers.ElementAt(i).Value);
                playerDetailsArray[i] = details;
                if (details.Name == PhotonNetwork.playerName)
                {
                    LocalPlayerID = i;
                }
            }

            CurrentPlayers = playerDetailsArray;

            if (_readyCheckCoroutine != null) _co.StopCoroutine(_readyCheckCoroutine);
            _readyCheckCoroutine = _co.StartCoroutine(PlayerReadyCountdown(READY_CHECK_TIME));
            SessionSeed = SeedGenerator.StringToSeed(PhotonNetwork.room.Name);
            EventDispatcher.Broadcast(RoomFilled, playerDetailsArray);
        }

        private void OnPedestalsConfigured(Pedestals pedestalData)
        {
            _pedestalData = pedestalData;
            SpawnPlayer();
        }

        private IEnumerator BroadcastSuccessfulReadyCheckDelayed(float delay)
        {
            _readyStateOK = true;   
            yield return new WaitForSecondsRealtime(delay);

            Debug.Log("Ready check finished");

            Models.Get<IGamestateModel>().SetState(Gamestate.StartingGame);
            Models.Get<IShuttersModel>().SetState(ShutterState.Open);
            
            if (_timerStartCoroutine != null) _co.StopCoroutine(_timerStartCoroutine);
            _timerStartCoroutine = _co.StartCoroutine(StartRoomTimerAsync(4));
        }
        
        private IEnumerator StartRoomTimerAsync(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            _photonRelay.StartRoomTimer(GAME_DURATION);
        }

        // Photon events

        private void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");

            if (_gamestate == Gamestate.Matchmaking)
            {
                _co.StartCoroutine(PostConnectToMaster(new TimeSpan(0, 0, POST_READY_WAIT)));
            }
            else if (_gamestate == Gamestate.ReadyCheckFailed)
            {
                PhotonNetwork.Disconnect();
            }
        }

        private void OnJoinedLobby()
        {
            Debug.Log("Connected to Lobby");
            EventDispatcher.Broadcast(NetworkEvent, NetEvent.ConnectedToLobby);
        }

        private void OnCreatedRoom()
        {
            Debug.Log("Created Room");

            EventDispatcher.Broadcast(NetworkEvent, NetEvent.CreatedRoom);

            if (PhotonNetwork.room == null) PhotonNetwork.JoinRoom(_lastRoom);
        }

        private void OnJoinedRoom()
        {
            if (_gamestate == Gamestate.Game || _gamestate == Gamestate.Postgame) return;

            Debug.Log($"Connected to Room: {PhotonNetwork.room.Name}");

            EventDispatcher.Broadcast(NetworkEvent, NetEvent.JoinedRoom);
            EventDispatcher.Broadcast(NetRoomEvent, RoomEvent.PlayerJoined, PhotonNetwork.room.PlayerCount.ToString());
            _lastRoom = PhotonNetwork.room.Name;

            if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
            {
                OnRoomFilled();
            }
        }

        private void OnReceivedRoomListUpdate()
        {
            _roomList = new List<RoomInfo>(PhotonNetwork.GetRoomList());

            Debug.Log("Room list was updated");

            EventDispatcher.Broadcast(NetworkEvent, NetEvent.RoomListUpdated);

            if (PhotonNetwork.room == null) JoinOrCreateRoom();
        }

        private void OnJoinRoomFailed(object[] args)
        {
            Debug.Log($"Failed to join room: [{args[0]}] {args[1]}");

            if (args[0].ToString() == "32746")
            {
                _roomBlacklist.Add(_lastRoom);
            }

            _lastRoom = string.Empty;
            PhotonNetwork.JoinLobby(_lobby);
        }

        private void OnPlayerConnected(string name)
        {
            if (_gamestate == Gamestate.Game || _gamestate == Gamestate.Postgame) return;

            Debug.Log($"Player [{name}] connected");

            EventDispatcher.Broadcast(NetRoomEvent, RoomEvent.PlayerJoined, name);

            if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
            {
                OnRoomFilled();
            }
        }

        private void OnPlayerDisconnected(string name)
        {
            Debug.Log($"Player [{name}] disconnected");
            EventDispatcher.Broadcast(NetRoomEvent, RoomEvent.PlayerDisconnected, name);

            if (_gamestate == Gamestate.ReadyCheckFailed || _gamestate == Gamestate.ReadyCheck)
            {
                CurrentErrorMessage = "Game could not start because another player failed the ready check.";
                OnJoinedRoom();
            }
        }

        private void OnReadyCheckFinished()
        {  
            if (_gamestate == Gamestate.ReadyCheck)
            {
                _co.StartCoroutine(BroadcastSuccessfulReadyCheckDelayed(0.1f));

                if (PhotonNetwork.isMasterClient)
                {
                    var htable = new ExitGames.Client.Photon.Hashtable();
                    htable.Add("gamestarted", true);
                    PhotonNetwork.room.SetCustomProperties(htable);
                    LocalPlayerStatus = PlayerStatus.Active;
                }
            }
        }
        
        private void OnDisconnectedFromPhoton()
        {
            if (_gamestate == Gamestate.ReadyCheckFailed)
            {
                Models.Get<ILoginUIModel>().RequestReset();
                Models.Get<IGamestateModel>().SetState(Gamestate.MainMenu);
                Models.Get<IShuttersModel>().SetState(ShutterState.Open);
            }
            else
            {
                Models.Get<IArenaModel>().PrepareForNewGame();
            }
        }

        private void OnLeftRoom()
        {
            if (_gamestate == Gamestate.ReadyCheckFailed)
            {
                Models.Get<IModalUIModel>().Hide();
            }
        }

        private void OnGameFinished()
        {
            var highestHealth = 0;
            foreach (var player in PhotonNetwork.playerList)
            {
                highestHealth = Mathf.Max(player.GetScore(), highestHealth);
            }

            if (PhotonNetwork.player.GetScore() != highestHealth)
            {
                LocalPlayerStatus = PlayerStatus.Defeated;
                Models.Get<IDBModel>().AddLoss(_currentUser);
            }
            else
            {
                var sortedPlayers = from player in PhotonNetwork.playerList
                    where player.GetScore() == highestHealth
                    select player;

                if (sortedPlayers.Count() > 1)
                {
                    LocalPlayerStatus = PlayerStatus.Tied;
                    Models.Get<IDBModel>().AddDraw(_currentUser);
                }
                else
                {
                    LocalPlayerStatus = PlayerStatus.Victorious;
                    Models.Get<IDBModel>().AddWin(_currentUser);
                }
            }

            _lastRoom = string.Empty;

            Models.Get<IGamestateModel>().SetState(Gamestate.Postgame);
        }

        private void OnPlayerDied(int photonPlayerID)
        {
            Models.Get<INetPlayerModel>().KillPlayer(photonPlayerID);
            
            if (PhotonNetwork.player.ID == photonPlayerID)
            {
                LocalPlayerStatus = PlayerStatus.Dead;
                Models.Get<IGamestateModel>().SetState(Gamestate.Postgame);
                Models.Get<IDBModel>().AddLoss(_currentUser);
            }
        }

        public void UpdatePlayerCameraTransform(int playerID, Transform transform)
        {
            CurrentPlayers[playerID].PlayerCameraTransform = transform;
            EventDispatcher.Broadcast(PlayerCameraTransformUpdated);
        }

        public void SetLocalPlayerStatus(PlayerStatus status)
        {
            LocalPlayerStatus = status;
            if (LocalPlayerStatus == PlayerStatus.Dead)
            {
                Models.Get<IGamestateModel>().SetState(Gamestate.Postgame);
                Models.Get<IDBModel>().AddLoss(_currentUser);
                _photonRelay.SendPlayerDeadEvent();
            }
        }

        public void DamagePlayer(int photonPlayerID, int amount)
        {
            if (PhotonNetwork.inRoom)
            {
                foreach (var player in PhotonNetwork.playerList)
                {
                    if (player.ID == photonPlayerID)
                    {
                        var newScore = Mathf.Max(player.GetScore() - amount, 0);
                        player.SetScore(newScore);

                        if (newScore == 0)
                        {
                            _photonRelay.SendPlayerDeadEvent();
                        }
                        break;
                    }
                }
            }
        }
    }
}
