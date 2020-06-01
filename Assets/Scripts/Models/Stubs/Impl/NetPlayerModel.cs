using System;
using System.Collections;
using UnityEngine;
using RR.Utility.Input;
using RR.Utility.Gameplay;
using RR.Utility;
using RR.Models.GamestateModel;
using RR.Models.NetworkModel;
using RR.Models.ArenaModel;
using RR.Models.PlayerCameraModel;
using RR.Models.ShuttersModel;
using RR.Models.OptionsUIModel;
using RR.Models.UIShakerModel;

namespace RR.Models.NetPlayerModel.Impl
{
    public class NetPlayerModel : Model, INetPlayerModel
    {
        public event Action<Vector2> PlayerMovement = delegate { };
        public event Action<Vector2> CameraLook = delegate { };
        public event Action<int> CameraZoom = delegate { };
        public event Action<RRAction> Action = delegate { };
        public event Action<bool> ProcessInputChanged = delegate { };
        public event Action ConnectCamera = delegate { };
        public event Action JumpEnd = delegate { };
        public event Action<PlayerDetails[], Pedestals> GameDetailsUpdated = delegate { };
        public event Action ReturnToPedestal = delegate { };
        public event Action Cleanup = delegate { };
        public event Action Death = delegate { };
        public event Action<int> KilledByNetPlayer = delegate { };

        private bool _processInputs;
        private bool _waitingForShutters;
        private bool _pendingDamage;
        private Gamestate _gamestate;
        private PlayerDetails[] _playerDetails;
        private Pedestals _pedestalDetails;
        private CoroutineOwner _co;
        private Coroutine _returnToPedestalCoroutine;
        private bool _dead;

        public void Initialize()
        {
            _processInputs = false;
            _pendingDamage = false;

            var go = new GameObject("NetPlayerModel_CoroutineOwner");
            _co = go.AddComponent<CoroutineOwner>();

            Models.Get<IGamestateModel>().StateChanged += OnGamestateChange;
            Models.Get<INetworkModel>().RoomFilled += OnRoomFilled;
            Models.Get<IArenaModel>().PedestalsConfigured += OnPedestalsConfigured;
            Models.Get<IShuttersModel>().ShutterTransitionEnd += OnShuttersTransitionEnd;
        }

        private void SetProcessInput(bool process, bool silent = false)
        {
            _processInputs = process;
            if (!silent) EventDispatcher.Broadcast(ProcessInputChanged, _processInputs);
        }

        // Updates
        public void SendInputFrame(InputFrame iframe)
        {
            if (Models.Get<IOptionsUIModel>().IsOpen) return;

            EventDispatcher.Broadcast(PlayerMovement, iframe.Move);
            EventDispatcher.Broadcast(CameraLook, iframe.Look);
            
            while (iframe.Keys.Count > 0)
            {
                var key = iframe.Keys.Dequeue();

                EventDispatcher.Broadcast(Action, key);
            }

            EventDispatcher.Broadcast(CameraZoom, iframe.Scroll);
        }

        public void RequestCameraReset()
        {
            Models.Get<IPlayerCameraModel>().RequestReset();
        }

        public void AlertViewInitialized()
        {
            EventDispatcher.Broadcast(GameDetailsUpdated, _playerDetails, _pedestalDetails);
        }

        public void Damage(int amount)
        {
            PhotonNetwork.player.SetScore(Mathf.Max(PhotonNetwork.player.GetScore() - amount, 0));

            if (PhotonNetwork.player.GetScore() == 0)
            {
                Models.Get<INetworkModel>().SetLocalPlayerStatus(PlayerStatus.Dead);
                EventDispatcher.Broadcast(Death);
            }
        }

        public void AlertOutOfBounds()
        {
            _waitingForShutters = true;
            _pendingDamage = true;
            Models.Get<IShuttersModel>().SetState(ShutterState.Closed, true);
            SetProcessInput(false);
        }

        private IEnumerator ReturnToPedestalAsync()
        {
            if (_pendingDamage) Damage(20);
            _pendingDamage = false;

            if (PhotonNetwork.player.GetScore() > 0)
            {
                EventDispatcher.Broadcast(ReturnToPedestal);
            }

            yield return new WaitForSeconds(0.3f);
            
            Models.Get<IShuttersModel>().SetState(ShutterState.Open, false);
            SetProcessInput(true);
        }

        public void PreDisconnectCleanup()
        {
            SetProcessInput(false, true);
            EventDispatcher.Broadcast(Cleanup);
        }

        public void DamagePlayer(int photonPlayerID, int amount)
        {
            Models.Get<INetworkModel>().DamagePlayer(photonPlayerID, amount);
        }

        public void KillPlayer(int photonPlayerID)
        {
            EventDispatcher.Broadcast(KilledByNetPlayer, photonPlayerID);
        }

        public void RequestUIShake()
        {
            Models.Get<IUIShakerModel>().Shake();
        }

        // Events
        private void OnGamestateChange(Gamestate state)
        {
            if (_gamestate != state)
            {
                _gamestate = state;

                if (_gamestate == Gamestate.Game)
                {
                    SetProcessInput(true);
                }
            }
        }

        private void OnRoomFilled(PlayerDetails[] playerDetails)
        {
            _playerDetails = playerDetails;
        }

        private void OnPedestalsConfigured(Pedestals pedestals)
        {
            _pedestalDetails = pedestals;
        }

        private void OnShuttersTransitionEnd()
        {
            if (_waitingForShutters)
            {
                _waitingForShutters = false;

                if (_returnToPedestalCoroutine != null) _co.StopCoroutine(_returnToPedestalCoroutine);
                _returnToPedestalCoroutine = _co.StartCoroutine(ReturnToPedestalAsync());
            }
        }
    }
}
