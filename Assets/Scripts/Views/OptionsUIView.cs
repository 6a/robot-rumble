using RR.Presenters;
using RR.Properties;

namespace RR.Views
{
    public class OptionsUIView : BaseView<OptionsUIPresenter, OptionsUIProperties>
    {
        public override void Initialize()
        {
            AddListeners();
            Presenter.Initialize();

            Properties.InvertYAxisToggle.isOn = UnityEngine.PlayerPrefs.GetInt("invert-y", 1) == 1 ? true : false;
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.ActiveChanged += OnActiveChanged;

            Properties.ReturnButton.onClick.AddListener(OnReturnClicked);
            Properties.QuitButton.onClick.AddListener(OnQuitClicked);
            Properties.InvertYAxisToggle.onValueChanged.AddListener(OnInvertYAxisChanged);
        }

        private void RemoveListeners()
        {
            Presenter.ActiveChanged -= OnActiveChanged;
            
            Properties.ReturnButton.onClick.RemoveListener(OnReturnClicked);
            Properties.QuitButton.onClick.RemoveListener(OnQuitClicked);
            Properties.InvertYAxisToggle.onValueChanged.RemoveListener(OnInvertYAxisChanged);
        }

        private void OnReturnClicked()
        {
            Presenter.Return();
        }

        private void OnQuitClicked()
        {
            Presenter.Quit();
        }

        private void OnActiveChanged(bool active)
        {
            Properties.Wrapper.SetActive(active);
        }

        private void OnInvertYAxisChanged(bool isOn)
        {
            Presenter.InvertYAxisChanged(isOn);
        }
    }
}
