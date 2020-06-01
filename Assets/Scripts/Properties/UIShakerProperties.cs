using UnityEngine;

namespace RR.Properties
{
    public class UIShakerProperties : BaseProperties
    {
        public RectTransform[] UIWrappers;
        public float Duration = 0.2f;
        public float Magnitude = 0.2f;
        public Vector2 Variation = new Vector2(0.4f, 0.5f);
        public float YMax = 135f;
        public float XMax = 240f;
    }
}
