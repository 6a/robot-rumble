using System;
using RR.Models.ArenaModel;
using RR.Models.NetworkModel;

namespace RR.Presenters
{
    public class ArenaPresenter : BasePresenter
    {
        public event Action<PlayerDetails> GameStart = delegate { };
        public event Action<int> NewSessionSeed = delegate { };
        public event Action<PlayerDetails> PlayerCameraTransformUpdated = delegate { };
        public event Action EnablePostGameCamera = delegate { };
        public event Action Reset = delegate { };

        public int Rings { get; private set; }
        public float Radius { get; private set; }

        public override void Initialize()
        {
            Models.Get<IArenaModel>().GameStart += OnGameStart;
            Models.Get<IArenaModel>().Reset += OnReset;
            Models.Get<IArenaModel>().NewSessionSeed += OnNewSessionSeed;
            Models.Get<IArenaModel>().PlayerCameraTransformUpdated += OnPlayerCameraTransformUpdated;
            Models.Get<IArenaModel>().EnablePostGameCamera += OnEnablePostGameCamera;
        }
        
        public override void Dispose()
        {
            Models.Get<IArenaModel>().GameStart -= OnGameStart;
            Models.Get<IArenaModel>().Reset -= OnReset;
            Models.Get<IArenaModel>().NewSessionSeed -= OnNewSessionSeed;
            Models.Get<IArenaModel>().PlayerCameraTransformUpdated -= OnPlayerCameraTransformUpdated;
            Models.Get<IArenaModel>().EnablePostGameCamera -= OnEnablePostGameCamera;
        }

        public void SetRings(int rings)
        {
            Rings = rings;
            Models.Get<IArenaModel>().SetRings(rings);
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
            Models.Get<IArenaModel>().SetRadius(radius);
        }

        void OnGameStart(PlayerDetails playerDetails)
        {
            EventDispatcher.Broadcast(GameStart, playerDetails);
        }

        void OnNewSessionSeed(int seed)
        {
            EventDispatcher.Broadcast(NewSessionSeed, seed);
        }

        void OnReset()
        {
            EventDispatcher.Broadcast(Reset);
        }

        private void OnPlayerCameraTransformUpdated(PlayerDetails playerDetails)
        {
            EventDispatcher.Broadcast(PlayerCameraTransformUpdated, playerDetails);
        }

        private void OnEnablePostGameCamera()
        {
            EventDispatcher.Broadcast(EnablePostGameCamera);
        }

        // Updates from view
        public void AlertPedestalsConfigured(Pedestals pedestalData)
        {
            Models.Get<IArenaModel>().AlertPedestalsConfigured(pedestalData);
        }

        public void AlertGenerationComplete()
        {
            Models.Get<IArenaModel>().AlertGenerationComplete();
        }

        public void AlertCameraSwitchReady()
        {
            Models.Get<IArenaModel>().AlertCameraSwitchReady();
        }
    }
}
