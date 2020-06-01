using UnityEngine;
using UnityEngine.UI;

namespace RR.Utility
{
    [RequireComponent(typeof(Image))]
    public class NoAlphaTestOnTransparency : MonoBehaviour
    {
        [SerializeField] private float _alphaHitTestMinimumThreshold = 0.2f;

        private void Awake()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = _alphaHitTestMinimumThreshold;
        }
    }
}