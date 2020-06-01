using System;
using UnityEngine;
using RR.Models.NetworkModel;

namespace RR.Models.PlayerCameraModel
{
    public interface IPlayerCameraModel : IModel
    {
        event Action<Vector2> LookUpdate;
        event Action<int> ZoomUpdate;
        event Action SetLocalPlayerCamera;
        event Action ActivateLocalPlayerCamera;
        event Action Reset;
        event Action<bool> InvertYChanged;

        Transform PlayerCameraTransform { get; }

        void SetCameraTransformReference(Transform transform);
        void SetPhotonView(PhotonView photonView);
        void ActivatePlayerCamera();
        void RequestReset();
        void SetInvertYAxis(bool invert);
    }
}

