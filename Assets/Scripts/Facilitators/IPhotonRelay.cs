using UnityEngine;
using System;

namespace RR.Facilitators.Photon
{
    public interface IPhotonRelay : IFacilitatorBase
    {
        event Action ConnectedToMaster;
        event Action JoinedLobby;
        event Action CreatedRoom;
        event Action JoinedRoom;
        event Action ReceivedRoomListUpdate;
        event Action<string> PlayerConnected;
        event Action<string> PlayerDisconnected;
        event Action<object[]> JoinRoomFailed;
        event Action ReadyCheckFinished;
        event Action Disconnected;
        event Action LeftRoom;
        event Action GameFinished;
        event Action<int> PlayerDied;

        void PlayerReady(bool isFromNetwork = false);
        void ResetReadyCheck();
        void StartRoomTimer(float duration);
        void SendPlayerDeadEvent();
    }
}
