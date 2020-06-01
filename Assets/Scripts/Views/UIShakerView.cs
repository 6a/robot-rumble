using RR.Presenters;
using RR.Properties;
using RR.Utility;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace RR.Views
{
    public class UIShakerView : BaseView<UIShakerPresenter, UIShakerProperties>
    {
        private List<Vector2> _shutterWrapperOrigins = new List<Vector2>();

        private Coroutine _shakeCoroutine;

        public override void Initialize()
        {
            Presenter.Initialize();

            foreach (var wrapper in Properties.UIWrappers)
            {
                _shutterWrapperOrigins.Add(wrapper.anchoredPosition);
            }

            _shakeCoroutine = null;
            
            AddListeners();
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.ShakeStart += Shake;
        }

        private void RemoveListeners()
        {
            Presenter.ShakeStart -= Shake;
        }

        private void AbortRunningShakes()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }
        }

        private void Shake()
        {
            AbortRunningShakes();
            _shakeCoroutine = StartCoroutine(ShakeAsync());
        }

        IEnumerator ShakeAsync()
        {
            for (int i = 0; i < Properties.UIWrappers.Length; i++)
            {
                Properties.UIWrappers[i].anchoredPosition = _shutterWrapperOrigins[i];
            }

            var firstPoint = new Vector2(-Properties.Variation.x, Random.Range(-Properties.Variation.y, Properties.Variation.y));
            var secondPoint = firstPoint * -1;
            var currentLerp = 0f;

            while (currentLerp < 1)
            {
                const float ONETHIRD = 1f / 3f;
                const float TWOTHIRDS = 2f / 3f;

                var delta =  (1 / (Properties.Duration / Time.deltaTime));
                currentLerp = Mathf.Clamp(currentLerp + delta, 0, 1);
                var quadLerp = Interpolation.EaseOutQuad(currentLerp, 0, 1);

                for (int i = 0; i < Properties.UIWrappers.Length; i++)
                {
                    Vector2 offset;
                    if (quadLerp < ONETHIRD)
                    {
                        offset = Vector2.Lerp(_shutterWrapperOrigins[i], firstPoint, quadLerp * 3);
                    }
                    else if (quadLerp < TWOTHIRDS)
                    {
                        offset = Vector2.Lerp(firstPoint, secondPoint, (quadLerp - ONETHIRD) * 3);
                    }
                    else
                    {
                        offset = Vector2.Lerp(secondPoint, _shutterWrapperOrigins[i], (quadLerp - TWOTHIRDS) * 3);
                    }

                    offset.x *= Properties.XMax * Properties.Magnitude;
                    offset.y *= Properties.YMax * Properties.Magnitude;

                    Properties.UIWrappers[i].anchoredPosition = offset;
                }

                yield return null;
            }

            Presenter.NotifyShakeEnd();
        }
    }
}
