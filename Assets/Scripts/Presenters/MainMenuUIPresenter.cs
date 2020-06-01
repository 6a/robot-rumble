using System;
using System.Collections.Generic;
using RR.Models.MainMenuUIModel;

namespace RR.Presenters
{
    public class MainMenuUIPresenter : BasePresenter
    {
        public event Action<MenuInterface> TransitionStarted = delegate { };
        public event Action<bool> ToggleVisibility = delegate { };

        private bool _visible;

        public override void Initialize()
        {
            Models.Get<IMainMenuUIModel>().TransitionStarted += OnTransitionStarted;
            Models.Get<IMainMenuUIModel>().ToggleVisibility += OnToggleVisibility;
            _visible = true;
        }
        
        public override void Dispose()
        {
            Models.Get<IMainMenuUIModel>().TransitionStarted -= OnTransitionStarted;
            Models.Get<IMainMenuUIModel>().ToggleVisibility -= OnToggleVisibility;
        }

        // Interaction notifiers
        public void AlertTransitionFinished()
        {
            Models.Get<IMainMenuUIModel>().AlertTransitionFinished();
        }

        // Event listeners
        private void OnTransitionStarted(MenuInterface ci)
        {
            EventDispatcher.Broadcast(TransitionStarted, ci);
        }
        
        private void OnToggleVisibility(bool visible)
        {
            if (_visible != visible)
            {
                _visible = visible;
                EventDispatcher.Broadcast(ToggleVisibility, _visible);
            }
        }
    }
}
