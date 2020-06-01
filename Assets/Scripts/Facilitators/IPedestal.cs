using UnityEngine;

namespace RR.Facilitators.Platform
{
    public interface IPedestal : IFacilitatorBase
    {
        Vector3 HexCoords { get; set; }
        Transform Transform { get; }
        Quaternion Facing { get; }

        void SetColor(Color color);
        Vector3 GetSpawnPosition();
        Vector3 GetBounds();
    }
}
