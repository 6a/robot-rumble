using RR.Presenters;
using RR.Properties;
using RR.Facilitators.Input;
using RR.Utility.Input;
using RR.Models.DBModel;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RR.Views
{
    public class CreateAccountUIView : BaseView<CreateAccountUIPresenter, CreateAccountUIProperties>
    {
        bool _interactable;

        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();

            _interactable = false;
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Properties.Create.onClick.AddListener(OnCreateClicked);
            Properties.Username.onValueChanged.AddListener(OnUsernameValueChanged);
            Properties.Password.onValueChanged.AddListener(OnPasswordValueChanged);
            Properties.Cancel.GetComponent<IInteractiveText>().Click += OnCancelClicked;

            Presenter.InteractableChanged += OninteractableChanged;
            Presenter.UIInputEvent += OnUIInputEvent;
            Presenter.CreateAccountStarted += OnCreateAccountStarted;
            Presenter.CreateAccountCompleted += OnCreateAccountCompleted;
            Presenter.ClearForm += OnClearForm;
        }

        private void RemoveListeners()
        {
            Properties.Create.onClick.RemoveListener(OnCreateClicked);
            Properties.Username.onValueChanged.RemoveListener(OnUsernameValueChanged);
            Properties.Password.onValueChanged.RemoveListener(OnPasswordValueChanged);
            Properties.Cancel.GetComponent<IInteractiveText>().Click -= OnCancelClicked;

            Presenter.InteractableChanged -= OninteractableChanged;
            Presenter.UIInputEvent -= OnUIInputEvent;
            Presenter.CreateAccountStarted -= OnCreateAccountStarted;
            Presenter.CreateAccountCompleted -= OnCreateAccountCompleted;
            Presenter.ClearForm -= OnClearForm;
        }

        private void AutoToggleCreateButton()
        {
            var usernameFilled = Properties.Username.text != string.Empty;
            var passwordFilled = Properties.Password.text != string.Empty;

            Properties.Create.interactable =  usernameFilled && passwordFilled;
        }

        private void OnCancelClicked()
        {
            if (!_interactable) return;

            Presenter.UserClickedCancel();
        }

        private void OnCreateClicked()
        {
            if (!_interactable) return;

            Properties.Username.interactable = Properties.Password.interactable = Properties.Create.interactable = false;
            Properties.ErrorMessage.text = "";

            Presenter.UserClickedCreate(new User{ name = Properties.Username.text, password = Properties.Password.text });
        }

        private void OnSubmit()
        {
            if (!_interactable) return;

            var currentSelected = EventSystem.current.currentSelectedGameObject;
            if (!currentSelected) return;

            if (currentSelected.name == "Password" && Properties.Create.interactable)
            {
                OnCreateClicked();
            }
            else if (currentSelected.name == "Username")
            {
                OnTabNavigate(true);
            }
        }

        public void OnTabNavigate(bool forward)
        {
            if (!_interactable) return;

            if (!EventSystem.current.currentSelectedGameObject)
            {
                Properties.Username.Select();
                return;
            }

            var current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

            Selectable next = null;
            if (forward)
            {
                next = current.navigation.selectOnDown;
                if (next && !next.interactable)
                {
                    next = next.navigation.selectOnDown;
                }
            }
            else
            {
                next = current.navigation.selectOnUp;
                if (next && !next.interactable)
                {
                    next = next.navigation.selectOnUp;
                }
            }

            if (next != null)
            {
                next.Select();
            }
        }

        public void OnCancel()
        {
            if (!_interactable) return;

            Debug.Log("Create - Cancel");
        }

        public void OnCreateAccountStarted()
        {
            Properties.Spinner.gameObject.SetActive(true);
        }

        // updating view
        private void OninteractableChanged(bool interactable)
        {
            _interactable = interactable;
            if (!_interactable)
            {
                Properties.ErrorMessage.text = "";
            }
            else
            {
                Properties.Username.Select();
            }
        }

        private void OnUIInputEvent(Queue<RRUICommand> events)
        {
            foreach (var command in events)
            {
                switch (command)
                {
                    case RRUICommand.NavigateForward:
                    {
                        OnTabNavigate(true);
                        break;
                    }
                    case RRUICommand.NavigateBack:
                    {
                        OnTabNavigate(false);
                        break;
                    }
                    case RRUICommand.Exit:
                    {
                        
                        break;
                    }
                    case RRUICommand.Submit:
                    {
                        OnSubmit();
                        break;
                    }
                }
            }
        }

        private void OnCreateAccountCompleted(string message)
        {
            Properties.Spinner.gameObject.SetActive(false);

            if (message != string.Empty)
            {
                Properties.ErrorMessage.text = message;
                Properties.Username.interactable = Properties.Password.interactable = true;
                AutoToggleCreateButton();
            }
        }

        private void OnUsernameValueChanged(string value)
        {
            AutoToggleCreateButton();
        }

        private void OnPasswordValueChanged(string value)
        {
            AutoToggleCreateButton();
        }

        private void OnPasswordConfirmationValueChanged(string value)
        {
            AutoToggleCreateButton();
        }

        private void OnClearForm()
        {
            Properties.Username.text = string.Empty;
            Properties.Password.text = string.Empty;

            Properties.Username.interactable = Properties.Password.interactable = true;
            AutoToggleCreateButton();
        }
    }
}
