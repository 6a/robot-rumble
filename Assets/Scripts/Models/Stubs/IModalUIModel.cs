using System;

namespace RR.Models.ModalUIModel
{
    public enum ModalState
    {
        Hidden,
        Lobby,
        ConnectingToServer,
        SearchingForGame,
        WaitingForPlayers,
        ReadyCheck,
        ReadyCheckFailed
    }

    public interface IModalUIModel : IModel
    {
        event Action<ModalState> ModalStateChange;
        event Action<int> PlayerCountChangeDuringSearch;
        event Action<string> Notification;

        void UserClickedReady();
        void UserClickedPlay();
        void UserClickedLeaderboards();
        void Hide();
    }
}

