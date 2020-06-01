using System;
using RR.Models.DBModel;
using UnityEngine;

namespace RR.Models.NetworkModel
{
    public enum NetEvent
    {
        ConnectedToMaster,
        ConnectedToLobby,
        RoomListUpdated,
        CreatedRoom,
        JoinedRoom,
    }

    public enum RoomEvent
    {
        PlayerJoined,
        PlayerDisconnected,
        PlayerLeft
    }

    public enum PlayerStatus
    {
        Active,
        Dead,
        Defeated,
        Tied,
        Victorious
    }

    public struct PlayerDetails
    {
        public PlayerDetails(int photonID, int id, string name)
        {
            PhotonID = photonID;
            ID = id;
            Name = name;
            PlayerCameraTransform = null;
        }

        public int PhotonID { get; private set; }
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Transform PlayerCameraTransform { get; set; }
    }

    public interface INetworkModel : IModel
    {
        event Action<NetEvent> NetworkEvent;
        event Action<RoomEvent, string> NetRoomEvent;
        event Action<PlayerDetails[]> RoomFilled;
        event Action PlayerSpawned;
        event Action PlayerCameraTransformUpdated;

        int PlayerCount { get; }
        PlayerDetails[] CurrentPlayers { get; }
        int LocalPlayerID { get; }
        int SessionSeed { get; }
        string CurrentErrorMessage { get; }
        PlayerStatus LocalPlayerStatus { get; }

        void SetUserAccount(User credentials);
        void ConfirmUserReady();
        void UpdatePlayerCameraTransform(int playerID, Transform transform);
        void SetLocalPlayerStatus(PlayerStatus status);
        void DamagePlayer(int photonPlayerID, int amount);
    }
}

