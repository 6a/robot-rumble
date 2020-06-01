using UnityEngine;

namespace RR.Properties
{
    public class PlayerCameraProperties : BaseProperties
    {
        public Vector2 ZoomRange = new Vector2(2, 3);
        public Transform Boom;
        public Transform BoomRoot;
        public Camera Camera;
        public AudioListener AudioListener;
        public Vector2 LookSensitivity = new Vector2(10, 2);
        public float PitchSmooth;
        public float YawSmooth;
        public Transform LookAtTarget;
        public PhotonView PlayerPhotonView;
        public float KillFloorY;
    }
}
