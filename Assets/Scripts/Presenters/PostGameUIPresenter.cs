using System;
using RR.Models.PostGameUIModel;
using RR.Models.NetworkModel;

namespace RR.Presenters
{
    public class PostGameUIPresenter : BasePresenter
    {
        public event Action<bool, PlayerStatus> ActiveChanged = delegate { };

        public override void Initialize()
        {
            Models.Get<IPostGameUIModel>().ActiveChanged += OnActiveChanged;
        }
        
        public override void Dispose()
        {
            Models.Get<IPostGameUIModel>().ActiveChanged -= OnActiveChanged;
        }

        public void ReturnClicked()
        {
            Models.Get<IPostGameUIModel>().ReturnClicked();
        }

        private void OnActiveChanged(bool active, PlayerStatus playerStatus)
        {
            EventDispatcher.Broadcast(ActiveChanged, active, playerStatus);
        }
    }
}
