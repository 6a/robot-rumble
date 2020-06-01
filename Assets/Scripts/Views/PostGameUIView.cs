using RR.Presenters;
using RR.Properties;
using RR.Models.NetworkModel;
using UnityEngine.UI;
using UnityEngine;

namespace RR.Views
{
    public class PostGameUIView : BaseView<PostGameUIPresenter, PostGameUIProperties>
    {
        public override void Initialize()
        {
            AddListeners();
            Presenter.Initialize();
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Properties.ReturnButton.onClick.AddListener(OnReturnClicked);

            Presenter.ActiveChanged += OnActiveChanged;
        }

        private void RemoveListeners()
        {
            Properties.ReturnButton.onClick.RemoveListener(OnReturnClicked);

            Presenter.ActiveChanged -= OnActiveChanged;
        }

        // Events

        private void OnActiveChanged(bool active, PlayerStatus playerStatus)
        {
            Properties.Wrapper.SetActive(active);
            if (!active) return;
            
            var buttonColor = Properties.ButtonStandardColor;
            switch (playerStatus)
            {
                case PlayerStatus.Dead:
                {
                    Properties.DeathTextWrapper.gameObject.SetActive(true);
                    Properties.NonDeathGameEndText.gameObject.SetActive(false);
                    Properties.DeathText.color = Properties.DeathColor;
                    buttonColor = Properties.DeathColor;
                    
                    break;
                }
                case PlayerStatus.Defeated:
                {
                    Properties.DeathTextWrapper.gameObject.SetActive(false);
                    Properties.NonDeathGameEndText.gameObject.SetActive(true);
                    Properties.NonDeathGameEndText.text = "DEFEAT - GIT GUD";

                    break;
                }
                case PlayerStatus.Tied:
                {
                    Properties.DeathTextWrapper.gameObject.SetActive(false);
                    Properties.NonDeathGameEndText.gameObject.SetActive(true);
                    Properties.NonDeathGameEndText.text = "TIED GAME";

                    break;
                }
                case PlayerStatus.Victorious:
                {
                    Properties.DeathTextWrapper.gameObject.SetActive(false);
                    Properties.NonDeathGameEndText.gameObject.SetActive(true);
                    Properties.NonDeathGameEndText.text = "VICTORY";

                    break;
                }
            }

            Properties.ReturnButton.GetComponent<Image>().color = buttonColor;
        }

        private void OnReturnClicked()
        {
            Presenter.ReturnClicked();
        }
    }
}
