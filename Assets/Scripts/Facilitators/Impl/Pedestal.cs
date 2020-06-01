using UnityEngine;

namespace RR.Facilitators.Platform.Impl
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Pedestal : MonoBehaviour, IPedestal
    {
        public Vector3 HexCoords { get; set; }
        public Transform Transform { get { return transform; } }
        public int AssignedPlayerID { get; set; }

        public Quaternion Facing 
        { 
            get
            {
                var flatPosition = transform.position;
                flatPosition.y = 0;
                flatPosition.Normalize();

                var quat = Quaternion.LookRotation(Vector3.zero - flatPosition, Vector3.up);
                return quat;
            }
        }

        public void SetColor(Color color)
        {
            GetComponent<MeshRenderer>().materials[0].SetColor("_MainColor", color);
        }

        public Vector3 GetBounds()
        {
            return GetComponent<MeshRenderer>() .bounds.size;
        }

        public Vector3 GetSpawnPosition()
        {
            return transform.position + new Vector3(0, 0.01f, 0);
        }
    }
}
