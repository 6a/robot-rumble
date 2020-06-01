using RR.Presenters;
using RR.Properties;
using RR.Models.MainMenuUIModel;
using System.Collections;
using RR.Utility;
using UnityEngine;

namespace RR.Views
{
    public class MainMenuUIView : BaseView<MainMenuUIPresenter, MainMenuUIProperties>
    {
        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.TransitionStarted += OnTransitionStarted;
            Presenter.ToggleVisibility += OnToggleVisibility;
        }

        private void RemoveListeners()
        {
            Presenter.TransitionStarted -= OnTransitionStarted;
            Presenter.ToggleVisibility -= OnToggleVisibility;
        }

        private IEnumerator Transition(MenuInterface targetInterface, float duration = 1f)
        {
            var lerp = 0f;
            var startScrollPosition = Properties.ScrollView.horizontalNormalizedPosition;
            var targetScrollPosition = Properties.ScrollPositions[(int)targetInterface];
            while (lerp < 1)
            {
                var delta =  (1 / (duration / Time.deltaTime));
                lerp = Mathf.Clamp(lerp + delta, 0, 1);
                var ease = Interpolation.EaseInOutQuad(lerp, 0, 1);

                var newPos = Mathf.Lerp(startScrollPosition, targetScrollPosition, ease);
                Properties.ScrollView.horizontalNormalizedPosition = newPos;

                yield return null;
            }

            Presenter.AlertTransitionFinished();
        }

        // Updating view
        private void OnTransitionStarted(MenuInterface ci)
        {
            StartCoroutine(Transition(ci, 0.5f));
        }

        private void OnToggleVisibility(bool visible)
        {
            Properties.Wrapper.SetActive(visible);
        }
    }
}
