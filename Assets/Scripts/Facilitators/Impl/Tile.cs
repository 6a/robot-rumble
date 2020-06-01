using UnityEngine;

namespace RR.Facilitators.Platform.Impl
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Tile : MonoBehaviour, ITile
    {
        public Vector3 HexCoords { get; set; }

        public void SetColor(Color color)
        {
            GetComponent<MeshRenderer>().materials[0].SetColor("_MainColor", color);
            GetComponent<MeshRenderer>().materials[0].SetColor("_Color", color);
        }

        public Vector3 GetBounds()
        {
            return GetComponent<MeshRenderer>() .bounds.size;
        }
    }
}
