using System;
using UnityEngine;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.MainMenuUIModel;
using RR.Models.UIEventListenerModel;
using RR.Models.DBModel;
using RR.Models.LoginUIModel;

namespace RR.Models.CreateAccountUIModel.Impl
{
    public class CreateAccountUIModel : Model, ICreateAccountUIModel
    {
        public bool Interactable { get; private set; }

        public event Action<bool> InteractableChanged = delegate { };
        public event Action<Queue<RRUICommand>> UIInputEvent = delegate { };
        public event Action<Response> CreateAccountCompleted = delegate { };
        public event Action ClearForm = delegate { };

        private string _lastSubmittedUsername;

        public void Initialize()
        {   
            Models.Get<IUIEventListenerModel>().UIEventRelay += OnUIInputEvent;
            Models.Get<IDBModel>().AccountCreationResult += OnCreateAccountCompleted;
            Models.Get<IMainMenuUIModel>().TransitionStarted += OnTransitionStarted;
            Models.Get<IMainMenuUIModel>().TransitionFinished += OnTransitionFinished;

            Interactable = false;
        }

        public void UserClickedCreate(User credentials)
        {
            Interactable = false;
            EventDispatcher.Broadcast(InteractableChanged, Interactable);

            _lastSubmittedUsername = credentials.name;
            Models.Get<IDBModel>().CreateAccount(credentials);
        }

        public void UserClickedCancel()
        {
            Models.Get<IMainMenuUIModel>().SetCurrentInterface(MenuInterface.Login);
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
            if (Models.Get<IMainMenuUIModel>().CurrentInterface == MenuInterface.CreateAccount)
            {
                SetActive();
            }
            else
            {
                EventDispatcher.Broadcast(ClearForm);
            }
        }

        private void OnUIInputEvent(Queue<RRUICommand> events)
        {
            if (Interactable)
            {
                EventDispatcher.Broadcast(UIInputEvent, events);
            }
        }

        private void OnCreateAccountCompleted(Response response)
        {
            EventDispatcher.Broadcast(CreateAccountCompleted, response);
            if (response.Code != 204)
            {
                SetActive();
            }
            else
            {
                Models.Get<IMainMenuUIModel>().SetCurrentInterface(MenuInterface.Login);
                Models.Get<ILoginUIModel>().SetUsername(_lastSubmittedUsername);
            }
        }
    }
}
