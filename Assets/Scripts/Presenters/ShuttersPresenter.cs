using System;
using RR.Models.ShuttersModel;
using RR.Models.UIShakerModel;

namespace RR.Presenters
{
    public class ShuttersPresenter : BasePresenter
    {
        public event Action<ShutterState, bool> ShutterStateChanged = delegate { };
        public ShutterState State { get; private set; }

        public override void Initialize()
        {
            Models.Get<IShuttersModel>().StateChanged += OnShutterStateChanged;
            State = ShutterState.Open;
        }
        
        public override void Dispose()
        {
            Models.Get<IShuttersModel>().StateChanged -= OnShutterStateChanged;
        }

        public void SetState(ShutterState state)
        {
            Models.Get<IShuttersModel>().SetState(state);
        }

        public void NotifyShutterImpact()
        {
            Models.Get<IUIShakerModel>().Shake();
        }

        public void NotifyShutterTransitionEnd()
        {
            Models.Get<IShuttersModel>().NotifyShutterTransitionEnd();
        }

        // Events
        private void OnShutterStateChanged(ShutterState newState, bool shake)
        {
            EventDispatcher.Broadcast(ShutterStateChanged, newState, shake);
        }
    }
}
