using System;
using UnityEngine;
using RR.Utility.Input;
using RR.Models.NetPlayerModel;
using RR.Models.NetworkModel;
using RR.Models.ArenaModel;

namespace RR.Presenters
{
    public class NetPlayerPresenter : BasePresenter
    {
        public event Action<Vector2> PlayerMovement = delegate { };
        public event Action<RRAction> Action = delegate { };
        public event Action<bool> ProcessInputChanged = delegate { };
        public event Action ConnectCamera = delegate { };
        public event Action<PlayerDetails[], Pedestals> GameDetailsUpdated = delegate { };
        public event Action ReturnToPedestal = delegate { };
        public event Action Cleanup = delegate { };
        public event Action Death = delegate { };
        public event Action<int> KilledByNetPlayer = delegate { };

        public override void Initialize()
        {
            Models.Get<INetPlayerModel>().PlayerMovement += OnPlayerMovement;
            Models.Get<INetPlayerModel>().Action += OnAction;
            Models.Get<INetPlayerModel>().ProcessInputChanged += OnProcessInputChanged;
            Models.Get<INetPlayerModel>().ConnectCamera += OnConnectCamera;
            Models.Get<INetPlayerModel>().GameDetailsUpdated += OnGameDetailsUpdated;
            Models.Get<INetPlayerModel>().ReturnToPedestal += OnReturnToPedestal;
            Models.Get<INetPlayerModel>().Cleanup += OnCleanup;
            Models.Get<INetPlayerModel>().Death += OnDeath;
            Models.Get<INetPlayerModel>().KilledByNetPlayer += OnKilledByNetPlayer;

            Models.Get<INetPlayerModel>().AlertViewInitialized();
        }
        
        public override void Dispose()
        {
            Models.Get<INetPlayerModel>().PlayerMovement -= OnPlayerMovement;
            Models.Get<INetPlayerModel>().Action -= OnAction;
            Models.Get<INetPlayerModel>().ProcessInputChanged -= OnProcessInputChanged;
            Models.Get<INetPlayerModel>().ConnectCamera -= OnConnectCamera;
            Models.Get<INetPlayerModel>().GameDetailsUpdated -= OnGameDetailsUpdated;
            Models.Get<INetPlayerModel>().ReturnToPedestal -= OnReturnToPedestal;
            Models.Get<INetPlayerModel>().Cleanup -= OnCleanup;
            Models.Get<INetPlayerModel>().Death -= OnDeath;
            Models.Get<INetPlayerModel>().KilledByNetPlayer -= OnKilledByNetPlayer;
        }

        private void OnPlayerMovement(Vector2 delta)
        {
            EventDispatcher.Broadcast(PlayerMovement, delta);
        }

        private void OnAction(RRAction action)
        {
            EventDispatcher.Broadcast(Action, action);
        }

        private void OnProcessInputChanged(bool processInput)
        {
            EventDispatcher.Broadcast(ProcessInputChanged, processInput);
        }

        private void OnConnectCamera()
        {
            EventDispatcher.Broadcast(ConnectCamera);
        }

        private void OnGameDetailsUpdated(PlayerDetails[] playerDetails, Pedestals pedestals)
        {
            EventDispatcher.Broadcast(GameDetailsUpdated, playerDetails, pedestals);
        }

        private void OnReturnToPedestal()
        {
            EventDispatcher.Broadcast(ReturnToPedestal);
        }

        private void OnCleanup()
        {
            EventDispatcher.Broadcast(Cleanup);
        }

        private void OnDeath()
        {
            EventDispatcher.Broadcast(Death);
        }

        private void OnKilledByNetPlayer(int photonPlayerID)
        {
            EventDispatcher.Broadcast(KilledByNetPlayer, photonPlayerID);
        }

        // Updates from view
        public void SendInputFrame(InputFrame frame)
        {
            Models.Get<INetPlayerModel>().SendInputFrame(frame);
        }

        public void RequestCameraReset()
        {
            Models.Get<INetPlayerModel>().RequestCameraReset();
        }

        public void Damage(int amount)
        {
            Models.Get<INetPlayerModel>().Damage(amount);
        }

        public void AlertOutOfBounds()
        {
            Models.Get<INetPlayerModel>().AlertOutOfBounds();
        }

        public void DamagePlayer(int photonPlayerID, int amount)
        {
            Models.Get<INetPlayerModel>().DamagePlayer(photonPlayerID, amount);
        }

        public void RequestUIShake()
        {
            Models.Get<INetPlayerModel>().RequestUIShake();
        }
    }
}
