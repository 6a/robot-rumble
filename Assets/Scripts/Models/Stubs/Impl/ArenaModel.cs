using System;
using RR.Models.NetworkModel;
using RR.Models.GamestateModel;
using RR.Models.PlayerCameraModel;

namespace RR.Models.ArenaModel.Impl
{
    public class ArenaModel : Model, IArenaModel
    {
        public event Action<PlayerDetails> GameStart = delegate { };
        public event Action Reset = delegate { };
        public event Action<Pedestals> PedestalsConfigured = delegate { };
        public event Action<int> NewSessionSeed = delegate { };
        public event Action GenerationComplete = delegate { };
        public event Action<PlayerDetails> PlayerCameraTransformUpdated = delegate { };
        public event Action EnablePostGameCamera = delegate { };

        public int Rings { get; private set; }
        public float Radius { get; private set; }

        public void Initialize()
        {
            Models.Get<INetworkModel>().RoomFilled += OnRoomFilled;
            Models.Get<INetworkModel>().PlayerCameraTransformUpdated += OnPlayerDetailsTransformUpdated;
            Models.Get<IGamestateModel>().StateChanged += OnGamestateChanged;
        }

        public void SetRings(int rings)
        {
            Rings = rings;
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
        }

        public void AlertPedestalsConfigured(Pedestals pedestalData)
        {
            EventDispatcher.Broadcast(PedestalsConfigured, pedestalData);
        }

        private void OnRoomFilled(PlayerDetails[] playerDetails)
        {
            var localPlayerID = Models.Get<INetworkModel>().LocalPlayerID;
            var localPlayer = playerDetails[localPlayerID];

            EventDispatcher.Broadcast(NewSessionSeed, Models.Get<INetworkModel>().SessionSeed);
        }

        private void OnGamestateChanged(Gamestate state)
        {
            if (state == Gamestate.StartingGame)
            {
                var playerDetails = Models.Get<INetworkModel>().CurrentPlayers;
                var localPlayer = playerDetails[Models.Get<INetworkModel>().LocalPlayerID];
                EventDispatcher.Broadcast(GameStart, localPlayer);
            }
            else if (state == Gamestate.Postgame)
            {
                if (PhotonNetwork.player.GetScore() <= 0)
                {
                    EventDispatcher.Broadcast(EnablePostGameCamera);
                }
            }
        }

        public void OnPlayerDetailsTransformUpdated()
        {
            var localPlayerID = Models.Get<INetworkModel>().LocalPlayerID;
            var localPlayer = Models.Get<INetworkModel>().CurrentPlayers[localPlayerID];
            EventDispatcher.Broadcast(PlayerCameraTransformUpdated, localPlayer);
        }

        public void AlertGenerationComplete()
        {
            Models.Get<IGamestateModel>().SetState(Gamestate.Game);
        }

        public void AlertCameraSwitchReady()
        {
            Models.Get<IPlayerCameraModel>().ActivatePlayerCamera();
        }

        public void PrepareForNewGame()
        {
            EventDispatcher.Broadcast(Reset);
        }
    }
}
