using System;
using RR.Models.ModalUIModel;

namespace RR.Presenters
{
    public class ModalUIPresenter : BasePresenter
    {
        public event Action<ModalState> ModalStateChange = delegate { };
        public event Action<int> RoomPlayerCountChange = delegate { };
        public event Action<string> Notification = delegate { };

        public override void Initialize()
        {
            Models.Get<IModalUIModel>().ModalStateChange += OnModalStateChange;
            Models.Get<IModalUIModel>().PlayerCountChangeDuringSearch += OnRoomPlayerCountChange;
            Models.Get<IModalUIModel>().Notification += OnNotification;
        }
        
        public override void Dispose()
        {
            Models.Get<IModalUIModel>().ModalStateChange -= OnModalStateChange;
            Models.Get<IModalUIModel>().PlayerCountChangeDuringSearch -= OnRoomPlayerCountChange;
            Models.Get<IModalUIModel>().Notification -= OnNotification;
        }

        public void UserClickedReady()
        {
            Models.Get<IModalUIModel>().UserClickedReady();
        }

        public void UserClickedPlay()
        {
            Models.Get<IModalUIModel>().UserClickedPlay();
        }

        public void UserClickedLeaderboards()
        {
            Models.Get<IModalUIModel>().UserClickedLeaderboards();
        }

        private void OnModalStateChange(ModalState state)
        {
            EventDispatcher.Broadcast(ModalStateChange, state);
        }

        private void OnRoomPlayerCountChange(int count)
        {
            EventDispatcher.Broadcast(RoomPlayerCountChange, count);
        }

        private void OnNotification(string notification)
        {
            EventDispatcher.Broadcast(Notification, notification);
        }
    }
}
