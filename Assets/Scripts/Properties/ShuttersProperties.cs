using UnityEngine;
using RR.Models.ShuttersModel;

namespace RR.Properties
{
    public class ShuttersProperties : BaseProperties
    {
        public RectTransform ShutterWrapper;
        public RectTransform ShutterTop;
        public RectTransform ShutterBottom;
        public RectTransform UICanvas;
        public ShutterState InitialShutterState;
        public int InitialShutterLerp;
        public float TransitionDuration = 1f;
        public float PartialOpenPercentage = 0.45f;
    }
}
