using System;
using RR.Models.HUDModel;

namespace RR.Presenters
{
    public class HUDPresenter : BasePresenter
    {
        public event Action<bool> ActiveChanged = delegate { };
        public event Action Reset = delegate { };

        public override void Initialize()
        {
            Models.Get<IHUDModel>().ActiveChanged += OnActiveChanged;
            Models.Get<IHUDModel>().Reset += OnReset;
        }
        
        public override void Dispose()
        {
            Models.Get<IHUDModel>().ActiveChanged -= OnActiveChanged;
            Models.Get<IHUDModel>().Reset -= OnReset;
        }

        private void OnActiveChanged(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active);
        }

        private void OnReset()
        {
            EventDispatcher.Broadcast(Reset);
        }
    }
}
