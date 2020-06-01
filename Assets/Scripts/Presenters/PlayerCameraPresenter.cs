using System;
using UnityEngine;
using System.Collections;
using RR.Models.PlayerCameraModel;

namespace RR.Presenters
{
    public class PlayerCameraPresenter : BasePresenter
    {
        public event Action<Vector2> LookUpdate = delegate { };
        public event Action<int> ZoomUpdate = delegate { };
        public event Action SetAsLocalPlayerCamera = delegate { };
        public event Action ActivateLocalPlayerCamera = delegate { };
        public event Action Reset = delegate { };
        public event Action<bool> InvertYChanged = delegate { };

        private PhotonView _photonView;
        
        public override void Initialize()
        {
            Models.Get<IPlayerCameraModel>().LookUpdate += OnLookUpdate;
            Models.Get<IPlayerCameraModel>().ZoomUpdate += OnZoomUpdate;
            Models.Get<IPlayerCameraModel>().SetLocalPlayerCamera += OnSetLocalPlayerCamera;
            Models.Get<IPlayerCameraModel>().ActivateLocalPlayerCamera += OnActivateLocalPlayerCamera;
            Models.Get<IPlayerCameraModel>().Reset += OnReset;
            Models.Get<IPlayerCameraModel>().InvertYChanged += OnInvertYChanged;
        }
        
        public override void Dispose()
        {
            Models.Get<IPlayerCameraModel>().LookUpdate -= OnLookUpdate;
            Models.Get<IPlayerCameraModel>().ZoomUpdate -= OnZoomUpdate;
            Models.Get<IPlayerCameraModel>().SetLocalPlayerCamera -= OnSetLocalPlayerCamera;
            Models.Get<IPlayerCameraModel>().ActivateLocalPlayerCamera += OnActivateLocalPlayerCamera;
            Models.Get<IPlayerCameraModel>().Reset -= OnReset;
            Models.Get<IPlayerCameraModel>().InvertYChanged -= OnInvertYChanged;
        }

        public void SetCameraTransformReference(Transform transform)
        {
            Models.Get<IPlayerCameraModel>().SetCameraTransformReference(transform);
        }

        public void SetPhotonView(PhotonView photonView)
        {
            _photonView = photonView;
            Models.Get<IPlayerCameraModel>().SetPhotonView(photonView);
        }

        private void OnLookUpdate(Vector2 delta)
        {
            if (delta.magnitude > 0)
            {
                EventDispatcher.Broadcast(LookUpdate, delta);
            }
        }

        private void OnZoomUpdate(int delta)
        {
            if (delta != 0)
            {
                EventDispatcher.Broadcast(ZoomUpdate, delta);
            }
        }

        private void OnSetLocalPlayerCamera()
        {
            if (_photonView.isMine)
            {
                EventDispatcher.Broadcast(SetAsLocalPlayerCamera);
            }
        }

        private void OnActivateLocalPlayerCamera()
        {
            if (_photonView.isMine)
            {
                EventDispatcher.Broadcast(ActivateLocalPlayerCamera);
            }
        }

        private void OnReset()
        {
            if (_photonView.isMine)
            {
                EventDispatcher.Broadcast(Reset);
            }
        }

        private void OnInvertYChanged(bool invert)
        {
            EventDispatcher.Broadcast(InvertYChanged, invert);
        }
    }
}
