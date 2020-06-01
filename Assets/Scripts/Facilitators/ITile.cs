using UnityEngine;
using RR.Utility.Hex;

namespace RR.Facilitators.Platform
{
    public interface ITile : IFacilitatorBase
    {
        Vector3 HexCoords { get; set; }
        void SetColor(Color color);
        Vector3 GetBounds();
    }
}
