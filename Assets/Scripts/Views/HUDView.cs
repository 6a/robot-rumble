using RR.Presenters;
using RR.Properties;

namespace RR.Views
{
    public class HUDView : BaseView<HUDPresenter, HUDProperties>
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
            Presenter.ActiveChanged += OnActiveChanged;
            Presenter.Reset += OnReset;
        }

        private void RemoveListeners()
        {
            Presenter.ActiveChanged -= OnActiveChanged;
            Presenter.Reset -= OnReset;
        }

        // Events

        private void OnActiveChanged(bool active)
        {
            Properties.Wrapper.SetActive(active);
        }

        private void OnReset()
        {
            Properties.CountdownComponent.Reset();
            Properties.ScoreboardComponent.Reset();
        }
    }
}
