using System;
using RR.Models.LoginUIModel;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.DBModel;

namespace RR.Presenters
{
    public class LoginUIPresenter : BasePresenter
    {
        public event Action<bool> InteractableChanged = delegate { };
        public event Action<Queue<RRUICommand>> UIInputEvent = delegate { };
        public event Action<string> LoginCompleted = delegate { };
        public event Action<string> UsernameOverwrite = delegate { };
        public event Action Reset = delegate { };

        private bool _visible;

        public override void Initialize()
        {
            Models.Get<ILoginUIModel>().InteractableChanged += OnInteractableChanged;
            Models.Get<ILoginUIModel>().UIInputEvent += OnUIInputEvent;
            Models.Get<ILoginUIModel>().LoginCompleted += OnLoginCompleted;
            Models.Get<ILoginUIModel>().UsernameOverwrite += OnUsernameOverwrite;
            Models.Get<ILoginUIModel>().Reset += OnResetRequested;
        }
        
        public override void Dispose()
        {
            Models.Get<ILoginUIModel>().InteractableChanged -= OnInteractableChanged;
            Models.Get<ILoginUIModel>().UIInputEvent -= OnUIInputEvent;
            Models.Get<ILoginUIModel>().LoginCompleted -= OnLoginCompleted;
            Models.Get<ILoginUIModel>().UsernameOverwrite -= OnUsernameOverwrite;
            Models.Get<ILoginUIModel>().Reset -= OnResetRequested;

        }

        // Interaction notifiers
        public void UserClickedCreateAccount()
        {   
            Models.Get<ILoginUIModel>().UserClickedCreateAccount();
        }

        public void UserClickedLogin(User credentials)
        {
            Models.Get<ILoginUIModel>().UserClickedLogin(credentials);
        }

        // Event listeners
        private void OnInteractableChanged(bool interactable)
        {
            if (InteractableChanged != null)
            {
                EventDispatcher.Broadcast(InteractableChanged, interactable);
            }
        }

        private void OnUIInputEvent(Queue<RRUICommand> events)
        {
            EventDispatcher.Broadcast(UIInputEvent, events);
        }

        private void OnLoginCompleted(Response response)
        {
            EventDispatcher.Broadcast(LoginCompleted, response.Message);
        }

        private void OnUsernameOverwrite(string username)
        {
            EventDispatcher.Broadcast(UsernameOverwrite, username);
        }

        private void OnResetRequested()
        {
            EventDispatcher.Broadcast(Reset);
        }
    }
}
