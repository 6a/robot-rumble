using UnityEngine;
using RR.Presenters;
using RR.Properties;
using RR.Models.ModalUIModel;
using System.Collections;

namespace RR.Views
{
    public class ModalUIView : BaseView<ModalUIPresenter, ModalUIProperties>
    {
        private ModalState _state;
        private Coroutine _countdownCoroutine;

        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();

            OnModalStateChange(ModalState.Hidden);
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.ModalStateChange += OnModalStateChange;
            Presenter.RoomPlayerCountChange += OnPlayerCountChange;
            Presenter.Notification += OnNotification;

            Properties.ReadyButton.onClick.AddListener(OnReadyClicked);
            Properties.PlayButton.onClick.AddListener(OnPlayClicked);
            Properties.LeaderboardsButton.onClick.AddListener(OnLeaderboardsClicked);
        }

        private void RemoveListeners()
        {
            Presenter.ModalStateChange -= OnModalStateChange;
            Presenter.RoomPlayerCountChange -= OnPlayerCountChange;
            Presenter.Notification -= OnNotification;

            Properties.ReadyButton.onClick.RemoveListener(OnReadyClicked);
            Properties.PlayButton.onClick.RemoveListener(OnPlayClicked);
            Properties.LeaderboardsButton.onClick.RemoveListener(OnLeaderboardsClicked);
        }

        private void OnModalStateChange(ModalState state)
        {
            if (_state != state)
            {
                _state = state;

                switch (_state)
                {
                    case ModalState.Hidden:
                    {
                        Properties.Wrapper.SetActive(false);
                        Properties.MessageText.text = string.Empty;
                        Properties.MessageText.gameObject.SetActive(true);
                        Properties.Logo.gameObject.SetActive(false);
                        Properties.Spinner.gameObject.SetActive(false);
                        Properties.ReadyButton.gameObject.SetActive(false);
                        Properties.PlayButton.gameObject.SetActive(false);
                        Properties.LeaderboardsButton.gameObject.SetActive(false);
                        Properties.Countdown.gameObject.SetActive(false);
                        Properties.ReadyButton.interactable = false;
                        Properties.PlayButton.interactable = false;
                        Properties.LeaderboardsButton.interactable = false;
                        break;
                    }
                    case ModalState.Lobby:
                    {
                        Properties.Wrapper.SetActive(true);
                        Properties.MessageText.text = string.Empty;
                        Properties.MessageText.gameObject.SetActive(false);
                        Properties.Logo.gameObject.SetActive(true);
                        Properties.Spinner.gameObject.SetActive(false);
                        Properties.ReadyButton.gameObject.SetActive(false);
                        Properties.PlayButton.gameObject.SetActive(true);
                        Properties.LeaderboardsButton.gameObject.SetActive(true);
                        Properties.Countdown.gameObject.SetActive(false);
                        Properties.ReadyButton.interactable = true;
                        Properties.PlayButton.interactable = true;
                        Properties.LeaderboardsButton.interactable = true;
                        break;
                    }
                    case ModalState.ConnectingToServer:
                    {
                        Properties.Wrapper.SetActive(true);
                        Properties.MessageText.text = "Connecting to Server";
                        Properties.MessageText.gameObject.SetActive(true);
                        Properties.Logo.gameObject.SetActive(false);
                        Properties.Spinner.gameObject.SetActive(true);
                        Properties.ReadyButton.gameObject.SetActive(false);
                        Properties.PlayButton.gameObject.SetActive(false);
                        Properties.LeaderboardsButton.gameObject.SetActive(false);
                        Properties.Countdown.gameObject.SetActive(false);
                        Properties.ReadyButton.interactable = false;
                        Properties.PlayButton.interactable = false;
                        Properties.LeaderboardsButton.interactable = false;
                        break;
                    }
                    case ModalState.SearchingForGame:
                    {
                        Properties.Wrapper.SetActive(true);
                        Properties.MessageText.text = "Searching for Game";
                        Properties.MessageText.gameObject.SetActive(true);
                        Properties.Logo.gameObject.SetActive(false);
                        Properties.Spinner.gameObject.SetActive(true);
                        Properties.ReadyButton.gameObject.SetActive(false);
                        Properties.PlayButton.gameObject.SetActive(false);
                        Properties.LeaderboardsButton.gameObject.SetActive(false);
                        Properties.Countdown.gameObject.SetActive(false);
                        Properties.ReadyButton.interactable = false;
                        Properties.PlayButton.interactable = false;
                        Properties.LeaderboardsButton.interactable = false;
                        break;
                    }
                    case ModalState.WaitingForPlayers:
                    {
                        Properties.Wrapper.SetActive(true);
                        Properties.MessageText.text = "Waiting for players";
                        Properties.MessageText.gameObject.SetActive(true);
                        Properties.Logo.gameObject.SetActive(false);
                        Properties.Spinner.gameObject.SetActive(true);
                        Properties.ReadyButton.gameObject.SetActive(false);
                        Properties.PlayButton.gameObject.SetActive(false);
                        Properties.LeaderboardsButton.gameObject.SetActive(false);
                        Properties.Countdown.gameObject.SetActive(false);
                        Properties.ReadyButton.interactable = false;
                        Properties.PlayButton.interactable = false;
                        Properties.LeaderboardsButton.interactable = false;
                        break;
                    }
                    case ModalState.ReadyCheck:
                    {
                        Properties.Wrapper.SetActive(true);
                        Properties.MessageText.text = "Ready?";
                        Properties.MessageText.gameObject.SetActive(true);
                        Properties.Logo.gameObject.SetActive(false);
                        Properties.Spinner.gameObject.SetActive(false);
                        Properties.ReadyButton.gameObject.SetActive(true);
                        Properties.PlayButton.gameObject.SetActive(false);
                        Properties.LeaderboardsButton.gameObject.SetActive(false);
                        Properties.Countdown.gameObject.SetActive(true);
                        Properties.ReadyButton.interactable = true;
                        Properties.PlayButton.interactable = false;
                        Properties.LeaderboardsButton.interactable = false;

                        if (_countdownCoroutine != null) StopCoroutine(_countdownCoroutine);
                        _countdownCoroutine = StartCoroutine(DecrementCountdown(30));

                        break;
                    }
                }
            }
        }

        private void OnPlayerCountChange(int count)
        {
            if (_state == ModalState.WaitingForPlayers)
            {
                Properties.MessageText.text = $"Waiting for players\n{count}/4";
            }
        }

        private void OnReadyClicked()
        {
            Properties.ReadyButton.interactable = false;
            Properties.MessageText.text = "Waiting for other players";
            Presenter.UserClickedReady();
        }

        private void OnPlayClicked()
        {
            Presenter.UserClickedPlay();
        }

        private void OnLeaderboardsClicked()
        {
            Presenter.UserClickedLeaderboards();
        }

        private void OnNotification(string notification)
        {
            Debug.Log($"OnNotification({notification})");

            if (notification == string.Empty)
            {

            }
            else
            {
                
            }
        }

        private IEnumerator DecrementCountdown(int startTime)
        {
            float start = (float)startTime;
            Properties.Countdown.text = startTime.ToString("00");

            while (start >= 0)
            {
                start = Mathf.Max(start - Time.unscaledDeltaTime, 0);

                var time = Mathf.Floor(start).ToString("00");

                Properties.Countdown.text = time;

                yield return null;
            }
        }
    }
}
