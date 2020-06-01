using UnityEngine;
using System;
using RR.Utility;
using PH = Photon;
using System.Collections.Generic;

namespace RR.Facilitators.Photon.Impl
{
    public class PhotonRelay : PH.MonoBehaviour, IPhotonRelay
    {
        private const byte EVT_PLAYER_DEAD = 0x64;
        private const string CRP_START_TIME = "start-time";
        private const string CRP_GAME_FINISHED = "game-finished";
        private const string RPC_PLAYERREADY = "PlayerReady";

        public event Action ConnectedToMaster = delegate { };
        public event Action JoinedLobby = delegate { };
        public event Action JoinedRoom = delegate { };
        public event Action CreatedRoom = delegate { };
        public event Action ReceivedRoomListUpdate = delegate { };
        public event Action<object[]> JoinRoomFailed = delegate { };
        public event Action<string> PlayerConnected = delegate { };
        public event Action<string> PlayerDisconnected = delegate { };
        public event Action ReadyCheckFinished = delegate { };
        public event Action Disconnected = delegate { };
        public event Action LeftRoom = delegate { };
        public event Action GameFinished = delegate { };
        public event Action<int> PlayerDied = delegate { };

        private int opponentState = 0;
        private EventDispatcher _dispatcher = new EventDispatcher();
        private int _playersReady = 0;
        private float _deltaTime = 0.0f;
        private double _timerStartTime;
        private bool _timerRunning;
        private float _roomTimer;
        private bool _startTimeSynced;
        private List<int> _livePlayers = new List<int>();

        private Texture2D _bgText;

        private void Awake()
        {
            PhotonNetwork.OnEventCall += OnEvent;

            _bgText = MakeTex(1, 1, Color.black);
        }

        private void OnGUI() 
        {
            GUI.backgroundColor = Color.black;

            // PrintPUNState();
            // PrintRoomState();
            // PrintOpponentState();
            // PrintFPS();
        }

        private Texture2D MakeTex( int width, int height, Color col )
        {
            Color[] pix = new Color[width * height];
            for( int i = 0; i < pix.Length; ++i )
            {
                pix[ i ] = col;
            }
            Texture2D result = new Texture2D( width, height );
            result.SetPixels( pix );
            result.Apply();
            return result;
        }

        private void PrintPUNState()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.padding = new RectOffset(2, 2, 2, 2);
            style.normal.background = _bgText;
            
            style.normal.textColor = Color.cyan;

            GUILayout.Label($"PUN status: {PhotonNetwork.connectionStateDetailed.ToString()}", style);
        }

        private void PrintOpponentState()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.padding = new RectOffset(2, 2, 2, 2);
            style.normal.background = _bgText;
            
            style.normal.textColor = Color.magenta;

            var opstate = "waiting for opponent";

            if (opponentState == 1) opstate = "connected";
            if (opponentState == 2) opstate = "disconnected";


            GUILayout.Label($"Opponent status: {opstate}", style);
        }

        private void PrintRoomState()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.padding = new RectOffset(2, 2, 2, 2);
            style.normal.background = _bgText;
            
            style.normal.textColor = Color.yellow;

            var name = PhotonNetwork.room != null ? PhotonNetwork.room.Name.ToString() : "none";
            var clientsCurrent = PhotonNetwork.room != null ? PhotonNetwork.room.PlayerCount.ToString() : "*";
            var clientsMax = PhotonNetwork.room != null ? PhotonNetwork.room.MaxPlayers.ToString() : "*";
            GUILayout.Label($"PUN room: {name} | Clients: {clientsCurrent}/{clientsMax}", style);
        }

        private void PrintFPS()
        {
            int w = Screen.width, h = Screen.height;
 
            GUIStyle style = new GUIStyle();
            style.fontSize = 12;
            style.padding = new RectOffset(2, 2, 2, 2);
            style.normal.background = _bgText;
            
            style.normal.textColor = Color.green;

            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUILayout.Label(text, style);
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void FixedUpdate()
        {
            if (_timerRunning)
            {
                GetAccurateStartTime();

                var remainingTime = System.Math.Max(_roomTimer - (PhotonNetwork.time - _timerStartTime), 0);
                if (remainingTime == 0 || GameFinishFlagSet())
                {
                    _dispatcher.Broadcast(GameFinished);
                    _timerRunning = false;

                    SetGameFinishedFlag();
                }
            }
        }

        private void GetAccurateStartTime()
        {
            if (!_startTimeSynced)
            {
                object prop = null;
                double startTime = 0;
                _startTimeSynced = PhotonNetwork.room.CustomProperties.TryGetValue(CRP_START_TIME, out prop);

                if (_startTimeSynced)
                {
                    startTime = (double)prop;
                    _timerStartTime = startTime;
                }
            }
        }

        private void SetGameFinishedFlag()
        {
            if (PhotonNetwork.inRoom)
            {
                var htable = new ExitGames.Client.Photon.Hashtable();
                htable.Add(CRP_GAME_FINISHED, true);
                PhotonNetwork.room.SetCustomProperties(htable);
            }
        }

        private bool GameFinishFlagSet()
        {
            var finished = false;
            if (PhotonNetwork.inRoom)
            {
                object prop = null;
                finished = PhotonNetwork.room.CustomProperties.TryGetValue(CRP_GAME_FINISHED, out prop);
            }

            return finished;
        }

        public void ResetReadyCheck()
        {
            _playersReady = 0;
        }

        public void OnPhotonPlayerDisconnected(PhotonPlayer player)
        {
            opponentState = 2;
            _dispatcher.Broadcast(PlayerDisconnected, player.NickName);
        }

        public void OnPhotonPlayerConnected(PhotonPlayer player)
        {
            opponentState = 1;
            _dispatcher.Broadcast(PlayerConnected, player.NickName);
        }

        public void OnConnectedToMaster()
        {
            _dispatcher.Broadcast(ConnectedToMaster);
        }

        public void OnJoinedLobby()
        {
            _dispatcher.Broadcast(JoinedLobby);
        }

        public void OnJoinedRoom()
        {
            _dispatcher.Broadcast(JoinedRoom);
            opponentState = PhotonNetwork.room.PlayerCount < 2 ? 0 : 1;
        }

        public void OnCreatedRoom()
        {
            _dispatcher.Broadcast(CreatedRoom);
        }

        public void OnReceivedRoomListUpdate()
        {
            _dispatcher.Broadcast(ReceivedRoomListUpdate);
        }

        public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            _dispatcher.Broadcast(JoinRoomFailed, codeAndMsg);
        }

        public void OnDisconnectedFromPhoton()
        {
            _dispatcher.Broadcast(Disconnected);
        }

        public void OnLeftRoom()
        {
            _dispatcher.Broadcast(LeftRoom);
        }

        public void OnEvent(byte eventCode, object content, int senderId)
        {
            switch (eventCode)
            {
                case EVT_PLAYER_DEAD:
                {
                    if (PhotonNetwork.inRoom)
                    {
                        var livePlayers = 0;
                        foreach (var player in PhotonNetwork.playerList)
                        {
                            if (player.GetScore() > 0)
                            {
                                livePlayers++;
                            }
                            else
                            {
                                _dispatcher.Broadcast(PlayerDied, player.ID);
                            }
                        }

                        if (livePlayers <= 1)
                        {
                            SetGameFinishedFlag();
                        }
                    }

                    break;
                }
            }
        }

        public void StartRoomTimer(float duration)
        {
            var now = PhotonNetwork.time;

            _timerRunning = true;
            _roomTimer = duration;
            _timerStartTime = now;

            if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient)
            {
                _startTimeSynced = true;

                var htable = new ExitGames.Client.Photon.Hashtable();
                htable.Add(CRP_START_TIME, now);
                PhotonNetwork.room.SetCustomProperties(htable);
            }
        }

        [PunRPC] public void PlayerReady(bool isFromNetwork = false)
        {
            if (!isFromNetwork)
            {
                photonView.RPC(RPC_PLAYERREADY, PhotonTargets.AllViaServer, true);
                return;
            }

            _playersReady++;
            if (_playersReady >= PhotonNetwork.room.MaxPlayers)
            {
                _dispatcher.Broadcast(ReadyCheckFinished);
                ResetReadyCheck();
            }
        }

        public void SendPlayerDeadEvent()
        {
            object[] content = new object[] { }; 
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
            PhotonNetwork.RaiseEvent(EVT_PLAYER_DEAD, content, true, raiseEventOptions);
        }
    }
}