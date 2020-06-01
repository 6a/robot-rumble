using System;
using UnityEngine;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.MainMenuUIModel;
using RR.Models.UIEventListenerModel;
using RR.Models.DBModel;
using RR.Models.ShuttersModel;
using RR.Models.GamestateModel;
using RR.Models.NetworkModel;

namespace RR.Models.LoginUIModel.Impl
{
    public class LoginUIModel : Model, ILoginUIModel
    {
        public bool Interactable { get; private set; }
        public User AuthenticatedUser { get; private set; }

        public event Action<bool> InteractableChanged = delegate { };
        public event Action<Queue<RRUICommand>> UIInputEvent = delegate { };
        public event Action<Response> LoginCompleted = delegate { };
        public event Action<string> UsernameOverwrite = delegate { };
        public event Action Reset = delegate { };

        private User _lastVerifiedCredentials;

        public void Initialize()
        {   
            Models.Get<IUIEventListenerModel>().UIEventRelay += OnUIInputEvent;
            Models.Get<IDBModel>().CredentialValidationResult += OnLoginCompleted;
            Models.Get<IMainMenuUIModel>().TransitionStarted += OnTransitionStarted;
            Models.Get<IMainMenuUIModel>().TransitionFinished += OnTransitionFinished;

            Interactable = true;
        }

        public void UserClickedLogin(User credentials)
        {
            Interactable = false;
            EventDispatcher.Broadcast(InteractableChanged, Interactable);

            _lastVerifiedCredentials = credentials;
            Models.Get<IDBModel>().ValidateCredentials(credentials);
        }

        public void UserClickedCreateAccount()
        {
            Models.Get<IMainMenuUIModel>().SetCurrentInterface(MenuInterface.CreateAccount);
        }

        public void SetUsername(string username)
        {
            EventDispatcher.Broadcast(UsernameOverwrite, username);
        }

        private void SetActive()
        {
            Interactable = true;
            EventDispatcher.Broadcast(InteractableChanged, Interactable);
        }

        private void OnTransitionStarted(MenuInterface ci)
        {
            if (Interactable) 
            {
                Interactable = false;
                EventDispatcher.Broadcast(InteractableChanged, Interactable);
            }
        }

        private void OnTransitionFinished()
        {
            if (Models.Get<IMainMenuUIModel>().CurrentInterface == MenuInterface.Login)
            {
                SetActive();
            }
        }

        private void OnUIInputEvent(Queue<RRUICommand> events)
        {
            if (Interactable)
            {
                EventDispatcher.Broadcast(UIInputEvent, events);
            }
        }

        private void OnLoginCompleted(Response response)
        {
            EventDispatcher.Broadcast(LoginCompleted, response);
            if (response.Code != 204)
            {
                SetActive();
                AuthenticatedUser = new User();
            }
            else
            {
                AuthenticatedUser = _lastVerifiedCredentials;
                Models.Get<IShuttersModel>().SetState(ShutterState.Closed);
                Models.Get<IGamestateModel>().SetState(Gamestate.Lobby);
                Models.Get<INetworkModel>().SetUserAccount(_lastVerifiedCredentials);
            }
        }

        public void RequestReset()
        {
            EventDispatcher.Broadcast(Reset);
            SetActive();
        }
    }
}
