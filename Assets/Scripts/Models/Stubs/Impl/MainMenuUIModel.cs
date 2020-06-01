using System;
using UnityEngine;
using RR.Models.GamestateModel;
using RR.Models.ShuttersModel;

namespace RR.Models.MainMenuUIModel.Impl
{
    public class MainMenuUIModel : Model, IMainMenuUIModel
    {   
        public event Action<MenuInterface> TransitionStarted = delegate { };
        public event Action TransitionFinished = delegate { };
        public event Action<bool> ToggleVisibility = delegate { };

        public MenuInterface CurrentInterface { get; private set; }

        private bool _transitioning;
        private MenuInterface _target;
        private Gamestate _gamestate;

        public void Initialize()
        {
            Models.Get<IGamestateModel>().StateChanged += OnGamestateChanged;
            Models.Get<IShuttersModel>().ShutterTransitionEnd += OnShuttersTransitionEnd;

            CurrentInterface = MenuInterface.Login;
            _transitioning = false;

            Models.Get<IGamestateModel>().SetState(Gamestate.MainMenu);
        }

        public void SetCurrentInterface(MenuInterface ci)
        {
            if (!_transitioning && ci != CurrentInterface)
            {
                _transitioning = true;
                _target = ci;
                EventDispatcher.Broadcast(TransitionStarted, CurrentInterface);
            }
        }

        public void AlertTransitionFinished()
        {
            CurrentInterface = _target;
            _transitioning = false;
            EventDispatcher.Broadcast(TransitionFinished);
        }

        private void OnShuttersTransitionEnd()
        {
            if (_gamestate == Gamestate.Lobby)
            {
                EventDispatcher.Broadcast(ToggleVisibility, false);
            }
        }

        private void OnGamestateChanged(Gamestate gs)
        {  
            if (_gamestate != gs)
            {
                _gamestate = gs;
                if (_gamestate == Gamestate.MainMenu)
                {
                    EventDispatcher.Broadcast(ToggleVisibility, true);
                }
            }
        }
    }
}
