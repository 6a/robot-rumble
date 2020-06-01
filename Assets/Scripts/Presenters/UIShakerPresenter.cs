using System;
using RR.Models.UIShakerModel;

namespace RR.Presenters
{
    public class UIShakerPresenter : BasePresenter
    {
        public event Action ShakeStart = delegate { };
        public event Action ShakeEnd = delegate { };

        public override void Initialize()
        {
            Models.Get<IUIShakerModel>().ShakeStart += OnShakeStart;
        }
        
        public override void Dispose()
        {
            Models.Get<IUIShakerModel>().ShakeStart -= OnShakeStart;
        }

        public void NotifyShakeEnd()
        {
            Models.Get<IUIShakerModel>().NotifyShakeEnd();
        }

        public void OnShakeStart()
        {
            EventDispatcher.Broadcast(ShakeStart);
        }
    }
}
