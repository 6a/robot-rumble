using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RR.Utility;

namespace RR.Facilitators.Gameplay
{
    [RequireComponent(typeof(Volume))]
    public class PPVolume : MonoBehaviour, IPPVolume
    {
        public static IPPVolume Instance { get; private set; }

        private Volume _volume;
        private ChromaticAberration _chromaObject = null;
        private Coroutine _chromaCoroutine;        

        private void Awake() 
        {
            Instance = this as IPPVolume;     
            _volume = GetComponent<Volume>();
            _volume.enabled = true;
            _volume.profile.TryGet<ChromaticAberration>(out _chromaObject);
        }

        public void ChromaPulse(float duration = 0.2f, float inToOutRatio = 0.2f)
        {
            if (_chromaCoroutine != null) StopCoroutine(_chromaCoroutine);
            _chromaCoroutine = StartCoroutine(ChromaPulseAsync(duration, inToOutRatio));
        }

        IEnumerator ChromaPulseAsync(float duration, float inToOutRatio)
        {
            var lerp = 0f;
            var intensity = 0f;
            _chromaObject.intensity.value = intensity;

            while (lerp < 1)
            {
                lerp = Mathf.Clamp01(lerp + (1 / (duration / Time.deltaTime)));
                
                if (lerp < inToOutRatio)
                {
                    intensity = Mathf.Clamp01(lerp / inToOutRatio);
                }
                else
                {
                    intensity = 1 - Interpolation.EaseInQuad(lerp, 0, 1);
                }

                _chromaObject.intensity.value = intensity;

                yield return null;
            }
        }
    }
}