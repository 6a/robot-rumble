using RR.Presenters;
using RR.Properties;
using RR.Utility;
using System.Collections;
using RR.Models.ShuttersModel;
using UnityEngine;

namespace RR.Views
{
    public class ShuttersView : BaseView<ShuttersPresenter, ShuttersProperties>
    {
        private float _currentShutterLerp;
        private float _shutterHeight;
        private ShutterState _state;

        private Coroutine _transitionCoroutine;

        public override void Initialize()
        {
            Presenter.Initialize();

            _shutterHeight = Properties.ShutterTop.rect.height;
            _currentShutterLerp = Properties.InitialShutterLerp;

            Properties.ShutterTop.anchoredPosition = new Vector2(Properties.ShutterTop.anchoredPosition.x, - (_shutterHeight * _currentShutterLerp));
            Properties.ShutterBottom.anchoredPosition = new Vector2(Properties.ShutterBottom.anchoredPosition.x, (_shutterHeight * _currentShutterLerp));

            _transitionCoroutine = null;

            AddListeners();

            Presenter.SetState(Properties.InitialShutterState);
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.ShutterStateChanged += SetShuttersState;
        }

        private void RemoveListeners()
        {
            Presenter.ShutterStateChanged -= SetShuttersState;
        }

        private void AbortRunningShutterTransitions()
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
        }

        private void SetShutterStateInstant(bool shake)
        {
            AbortRunningShutterTransitions();
            _transitionCoroutine = StartCoroutine(SetShuttersStateAsync(0, shake));
        }

        private void SetShuttersState(ShutterState newState, bool shake)
        {
            _state = newState;
            AbortRunningShutterTransitions();
            _transitionCoroutine = StartCoroutine(SetShuttersStateAsync(Properties.TransitionDuration, shake));
        }

        IEnumerator SetShuttersStateAsync(float duration, bool shake)
        {
            var lerpBounds = new Vector2();
            if (_state == ShutterState.Closed) lerpBounds = new Vector2(0, 1);
            else if (_state == ShutterState.Partial) lerpBounds = (_currentShutterLerp < Properties.PartialOpenPercentage) ? new Vector2(0, 1) : new Vector2(1, 0);
            else lerpBounds = new Vector2(1, 0);

            var directionMod = (lerpBounds.y - _currentShutterLerp > 0) ? 1 : -1;
            var targetHeight = (_state == ShutterState.Partial) ? (1 - Properties.PartialOpenPercentage) * _shutterHeight : _shutterHeight;
            var hasShaken = !shake;
            
            while (_currentShutterLerp != lerpBounds.y)
            {   
                yield return null;

                var delta =  (1 / (duration / Time.deltaTime)) * directionMod;

                _currentShutterLerp = Mathf.Clamp(_currentShutterLerp + delta, 0, 1);
                
                var bounceLerp = 0f;
                var translateDelta = 0f;

                if (directionMod == 1) 
                {
                    bounceLerp = Interpolation.EaseBounce(_currentShutterLerp, 0, 1);
                    translateDelta = bounceLerp * targetHeight;
                }
                else
                {
                    bounceLerp = Interpolation.EaseBounce(1 - _currentShutterLerp, 0, 1);
                    translateDelta = _shutterHeight - bounceLerp * targetHeight;
                }

                Properties.ShutterTop.anchoredPosition = new Vector2(Properties.ShutterTop.anchoredPosition.x, 0 - translateDelta);
                Properties.ShutterBottom.anchoredPosition = new Vector2(Properties.ShutterBottom.anchoredPosition.x, translateDelta);

                if (!hasShaken && bounceLerp > 0.95f)
                {
                    hasShaken = true;
                    Presenter.NotifyShutterImpact();
                }
            }

            yield return new WaitForEndOfFrame();
            Presenter.NotifyShutterTransitionEnd();
        }
    }
}
