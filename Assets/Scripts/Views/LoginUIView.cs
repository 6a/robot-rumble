using RR.Presenters;
using RR.Properties;
using RR.Facilitators.Input;
using RR.Utility.Input;
using RR.Models.DBModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RR.Views
{
    public class LoginUIView : BaseView<LoginUIPresenter, LoginUIProperties>
    {
        bool _interactable;

        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();

            _interactable = true;
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Properties.Login.onClick.AddListener(OnLoginClicked);
            Properties.Username.onValueChanged.AddListener(OnUsernameValueChanged);
            Properties.Password.onValueChanged.AddListener(OnPasswordValueChanged);
            Properties.CreateAccount.GetComponent<IInteractiveText>().Click += OnCreateAccountClicked;

            Presenter.InteractableChanged += OninteractableChanged;
            Presenter.UIInputEvent += OnUIInputEvent;
            Presenter.LoginCompleted += OnLoginCompleted;
            Presenter.UsernameOverwrite += OnUsernameOverwrite;
            Presenter.Reset += OnReset;
        }

        private void RemoveListeners()
        {
            Properties.Login.onClick.RemoveListener(OnLoginClicked);
            Properties.Username.onValueChanged.RemoveListener(OnUsernameValueChanged);
            Properties.Password.onValueChanged.RemoveListener(OnPasswordValueChanged);
            Properties.CreateAccount.GetComponent<IInteractiveText>().Click -= OnCreateAccountClicked;

            Presenter.InteractableChanged -= OninteractableChanged;
            Presenter.UIInputEvent -= OnUIInputEvent;
            Presenter.LoginCompleted -= OnLoginCompleted;
            Presenter.UsernameOverwrite -= OnUsernameOverwrite;
            Presenter.Reset -= OnReset;
        }

        private void OnCreateAccountClicked()
        {
            if (!_interactable) return;

            Presenter.UserClickedCreateAccount();
        }

        private void OnLoginClicked()
        {
            if (!_interactable) return;

            Properties.Username.interactable = Properties.Password.interactable = Properties.Login.interactable = false;
            Properties.ErrorMessage.text = "";
            Properties.Spinner.gameObject.SetActive(true);

            Presenter.UserClickedLogin(new User{ name = Properties.Username.text, password = Properties.Password.text });
        }

        private void OnSubmit()
        {
            if (!_interactable) return;

            var currentSelected = EventSystem.current.currentSelectedGameObject;
            if (!currentSelected) return;
            
            if (currentSelected.name == "Password" && Properties.Login.interactable)
            {
                OnLoginClicked();
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

            Debug.Log("Login - Cancel");
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

        private void OnLoginCompleted(string message)
        {
            Properties.Spinner.gameObject.SetActive(false);

            if (message != string.Empty)
            {
                Properties.ErrorMessage.text = message;
                Properties.Username.interactable = Properties.Password.interactable = true;
                AutoToggleLoginButton();
            }
        }

        private void OnUsernameValueChanged(string value)
        {
            AutoToggleLoginButton();
        }

        private void OnPasswordValueChanged(string value)
        {
            AutoToggleLoginButton();
        }

        private void OnUsernameOverwrite(string username)
        {
            if (username != string.Empty) 
            {
                Properties.Username.text = username;
                Properties.Password.text = "";
            }
        }

        private void OnReset()
        {
            Properties.Username.interactable = Properties.Password.interactable = true;
            Properties.Username.Select();
            AutoToggleLoginButton();
        }

        private void AutoToggleLoginButton()
        {
            Properties.Login.interactable = Properties.Username.text != string.Empty && Properties.Password.text != string.Empty;
        }
    }
}
