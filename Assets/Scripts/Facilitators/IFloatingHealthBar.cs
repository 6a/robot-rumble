using UnityEngine;

namespace RR.Facilitators.UI
{
    public interface IFloatingHealthBar
    {
        void Init(PhotonView photonView, Transform anchor);
        void Destroy();
    }
}