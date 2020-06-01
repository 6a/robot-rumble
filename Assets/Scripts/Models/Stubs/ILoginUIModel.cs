using System;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.DBModel;

namespace RR.Models.LoginUIModel
{
    public interface ILoginUIModel : IModel
    {
        bool Interactable { get; }
        User AuthenticatedUser { get; }

        event Action<bool> InteractableChanged;
        event Action<Queue<RRUICommand>> UIInputEvent;
        event Action<Response> LoginCompleted;
        event Action<string> UsernameOverwrite;
        event Action Reset;

        void UserClickedCreateAccount();
        void UserClickedLogin(User credentials);
        void SetUsername(string username);
        void RequestReset();
    }
}

