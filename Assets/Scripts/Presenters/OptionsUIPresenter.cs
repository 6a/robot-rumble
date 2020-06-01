using System;
using RR.Models.OptionsUIModel;

namespace RR.Presenters
{
    public class OptionsUIPresenter : BasePresenter
    {
        public event Action<bool> ActiveChanged = delegate { };

        public override void Initialize()
        {
            Models.Get<IOptionsUIModel>().ActiveChanged += OnActiveChanged;
        }
        
        public override void Dispose()
        {
            Models.Get<IOptionsUIModel>().ActiveChanged -= OnActiveChanged;
        }

        private void OnActiveChanged(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active);
        }

        public void Quit()
        {
            Models.Get<IOptionsUIModel>().Quit();
        }

        public void Return()
        {
            Models.Get<IOptionsUIModel>().Return();
        }

        public void InvertYAxisChanged(bool isOn)
        {
            Models.Get<IOptionsUIModel>().InvertYAxisChanged(isOn);
        }
    }
}
