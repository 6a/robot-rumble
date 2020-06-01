using System;

namespace RR.Models.MainMenuUIModel
{
    public enum MenuInterface
    {
        Login,
        CreateAccount
    }

    public interface IMainMenuUIModel : IModel
    {
        event Action<MenuInterface> TransitionStarted;
        event Action TransitionFinished;
        event Action<bool> ToggleVisibility;

        MenuInterface CurrentInterface { get; }
        
        void SetCurrentInterface(MenuInterface ci);
        void AlertTransitionFinished();
    }
}

