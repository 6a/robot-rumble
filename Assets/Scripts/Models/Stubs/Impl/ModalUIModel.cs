using System;
using RR.Models.GamestateModel;
using RR.Models.ShuttersModel;
using RR.Models.NetworkModel;
using RR.Models.LeaderboardsUIModel;

namespace RR.Models.ModalUIModel.Impl
{
    public class ModalUIModel : Model, IModalUIModel
    {
        public event Action<ModalState> ModalStateChange = delegate { };
        public event Action<int> PlayerCountChangeDuringSearch = delegate { };
        public event Action<string> Notification = delegate { };

        private Gamestate _gamestate;
        private ModalState _modalState;

        public void Initialize()
        {
            Models.Get<IShuttersModel>().ShutterTransitionEnd += OnShuttersTransitionEnd;
            Models.Get<IGamestateModel>().StateChanged += OnGamestateChanged;
            Models.Get<INetworkModel>().NetworkEvent += OnNetworkEvent;
            Models.Get<INetworkModel>().NetRoomEvent += OnNetworkRoomEvent;
            Models.Get<INetworkModel>().RoomFilled += OnRoomFilled;
        }

        private void SetState(ModalState state)
        {
            if (_modalState != state)
            {
                _modalState = state;
                EventDispatcher.Broadcast(ModalStateChange, _modalState);             
            }
        }

        public void UserClickedReady()
        {
            Models.Get<INetworkModel>().ConfirmUserReady();
        }

        public void UserClickedPlay()
        {
            SetState(ModalState.ConnectingToServer);
            Models.Get<IGamestateModel>().SetState(Gamestate.Matchmaking);
        }

        public void UserClickedLeaderboards()
        {
            Models.Get<ILeaderboardsUIModel>().SetActive(true);
        }

        public void Hide()
        {
            SetState(ModalState.Hidden);
        }

        private void OnNetworkEvent(NetEvent nevent)
        {
            switch (nevent)
            {
                case NetEvent.ConnectedToMaster:
                {
                    SetState(ModalState.SearchingForGame);
                    break;
                }
                case NetEvent.ConnectedToLobby:
                {
                    break;
                }
                case NetEvent.RoomListUpdated:
                {

                    break;
                }
                case NetEvent.CreatedRoom:
                {

                    break;
                }
                case NetEvent.JoinedRoom:
                {
                    SetState(ModalState.WaitingForPlayers);
                    break;
                }
            }

            if (Models.Get<INetworkModel>().CurrentErrorMessage != string.Empty)
            {
                EventDispatcher.Broadcast(Notification, Models.Get<INetworkModel>().CurrentErrorMessage);
            }
        }

        private void OnNetworkRoomEvent(RoomEvent revent, string message)
        {
            if (revent == RoomEvent.PlayerJoined || revent == RoomEvent.PlayerDisconnected)
            {
                var playerCount = Models.Get<INetworkModel>().PlayerCount;
                EventDispatcher.Broadcast(PlayerCountChangeDuringSearch, playerCount);
            }
        }

        private void OnRoomFilled(PlayerDetails[] playerDetails)
        {
            SetState(ModalState.ReadyCheck);
        }

        private void OnShuttersTransitionEnd()
        {
            if (_gamestate == Gamestate.Lobby)
            {
                SetState(ModalState.Lobby);
            }
        }

        private void OnGamestateChanged(Gamestate gs)
        {  
            if (_gamestate != gs)
            {
                _gamestate = gs;

                if (_gamestate == Gamestate.StartingGame || _gamestate == Gamestate.MainMenu)
                {
                    SetState(ModalState.Hidden);
                }
                else if (_gamestate == Gamestate.ReadyCheckFailed)
                {
                    SetState(ModalState.ReadyCheckFailed);
                }
            }
        }
    }
}
