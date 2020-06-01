using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.DBModel;
using RR.Models.CreateAccountUIModel;

namespace RR.Presenters
{
    public class CreateAccountUIPresenter : BasePresenter
    {
        public event Action<bool> InteractableChanged = delegate { };
        public event Action<Queue<RRUICommand>> UIInputEvent = delegate { };
        public event Action CreateAccountStarted = delegate { };
        public event Action<string> CreateAccountCompleted = delegate { };
        public event Action ClearForm = delegate { };

        public override void Initialize()
        {
            Models.Get<ICreateAccountUIModel>().InteractableChanged += OnInteractableChanged;
            Models.Get<ICreateAccountUIModel>().UIInputEvent += OnUIInputEvent;
            Models.Get<ICreateAccountUIModel>().CreateAccountCompleted += OnCreateAccountCompleted;
            Models.Get<ICreateAccountUIModel>().ClearForm += OnClearForm;
        }
        
        public override void Dispose()
        {
            Models.Get<ICreateAccountUIModel>().InteractableChanged -= OnInteractableChanged;
            Models.Get<ICreateAccountUIModel>().UIInputEvent -= OnUIInputEvent;
            Models.Get<ICreateAccountUIModel>().CreateAccountCompleted -= OnCreateAccountCompleted;
            Models.Get<ICreateAccountUIModel>().ClearForm -= OnClearForm;
        }

        // Interaction notifiers
        public void UserClickedCancel()
        {   
            Models.Get<ICreateAccountUIModel>().UserClickedCancel();
        }

        public void UserClickedCreate(User credentials)
        {
            if (credentials.name.Length < 2 || credentials.name.Length > 12)
            {
                OnInteractableChanged(true);
                EventDispatcher.Broadcast(CreateAccountCompleted, "Username must be between 2 and 12 characters long");
            } 
            else if (Regex.IsMatch(credentials.name, DBConstants.USERNAME_ILLEGAL_CHARS_REGEX)) 
            {
                OnInteractableChanged(true);
                EventDispatcher.Broadcast(CreateAccountCompleted, "Username can only contain letters and numbers");
            }
            else if (credentials.password.Length < 6 || credentials.password.Length > 20)
            {
                OnInteractableChanged(true);
                EventDispatcher.Broadcast(CreateAccountCompleted, "Password must be between 6 and 20 characters long");
            }
            else
            {
                Models.Get<ICreateAccountUIModel>().UserClickedCreate(credentials);
                EventDispatcher.Broadcast(CreateAccountStarted);
            }
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

        private void OnCreateAccountCompleted(Response response)
        {
            EventDispatcher.Broadcast(CreateAccountCompleted, response.Message);
        }

        private void OnClearForm()
        {
            EventDispatcher.Broadcast(ClearForm);
        }
    }
}
