using System;
using UnityEngine;
using RR.Models.NetPlayerModel;
using RR.Models.NetworkModel;

namespace RR.Models.PlayerCameraModel.Impl
{
    public class PlayerCameraModel : Model, IPlayerCameraModel
    {
        public event Action<Vector2> LookUpdate = delegate { };
        public event Action<int> ZoomUpdate = delegate { };
        public event Action SetLocalPlayerCamera = delegate { };
        public event Action ActivateLocalPlayerCamera = delegate { };
        public event Action Reset = delegate { };
        public event Action<bool> InvertYChanged = delegate { };

        public Transform PlayerCameraTransform { get; private set; }

        private bool _setPlayerCameraPending;
        private PhotonView _photonView;

        public void Initialize()
        {
            Models.Get<INetPlayerModel>().CameraLook += OnLookUpdate;
            Models.Get<INetPlayerModel>().CameraZoom += OnZoomUpdate;
            Models.Get<INetworkModel>().PlayerSpawned += OnPlayerSpawned;
        }

        // Updates from view
        public void SetCameraTransformReference(Transform transform)
        {
            PlayerCameraTransform = transform;
            Models.Get<INetworkModel>().UpdatePlayerCameraTransform(Models.Get<INetworkModel>().LocalPlayerID, PlayerCameraTransform);
        }

        public void SetPhotonView(PhotonView photonView)
        {
            _photonView = photonView;

            if (_setPlayerCameraPending)
            {
                _setPlayerCameraPending = false;
                EventDispatcher.Broadcast(SetLocalPlayerCamera);
            }
        }

        public void ActivatePlayerCamera()
        {
            EventDispatcher.Broadcast(ActivateLocalPlayerCamera);
        }

        public void RequestReset()
        {
            EventDispatcher.Broadcast(Reset);
        }

        public void SetInvertYAxis(bool invert)
        {
            EventDispatcher.Broadcast(InvertYChanged, invert);
        }

        // Events
        private void OnLookUpdate(Vector2 delta)
        {
            EventDispatcher.Broadcast(LookUpdate, delta);
        }

        private void OnZoomUpdate(int delta)
        {
            EventDispatcher.Broadcast(ZoomUpdate, delta);
        }

        private void OnPlayerSpawned()
        {
            _setPlayerCameraPending = true;
        }
    }
}
