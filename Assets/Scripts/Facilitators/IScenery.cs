using UnityEngine;
using RR.Utility;
using System.Collections.Generic;

namespace RR.Facilitators.Scenery
{
    public interface IScenery : IFacilitatorBase
    {
        void Init(Color color, Range yRange, float unitsPerSecond, Queue<IScenery> pool, List<IScenery> sceneryInMotion);
        void Animate();
    }
}
