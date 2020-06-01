using UnityEngine;

namespace RR.Utility
{
    [System.Serializable]
    public struct Range
    {
        public float Min, Max;

        public float GetRandom()
        {
            return Random.Range(Min, Max);
        }

        public float ClampToRange(float v)
        {
            return Mathf.Clamp(v, Min, Max);
        }
    }
}