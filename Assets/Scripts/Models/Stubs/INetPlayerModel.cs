using System;
using RR.Utility.Input;
using RR.Models.NetworkModel;
using RR.Models.ArenaModel;
using UnityEngine;

namespace RR.Models.NetPlayerModel
{
    public interface INetPlayerModel : IModel
    {
        event Action<Vector2> PlayerMovement;
        event Action<Vector2> CameraLook;
        event Action<int> CameraZoom;
        event Action<RRAction> Action;
        event Action<bool> ProcessInputChanged;
        event Action ConnectCamera;
        event Action<PlayerDetails[], Pedestals> GameDetailsUpdated;
        event Action ReturnToPedestal;
        event Action Cleanup;
        event Action Death;
        event Action<int> KilledByNetPlayer;

        void AlertViewInitialized();
        void SendInputFrame(InputFrame iframe);
        void RequestCameraReset();
        void Damage(int amount);
        void AlertOutOfBounds();
        void PreDisconnectCleanup();
        void DamagePlayer(int photonPlayerID, int amount);
        void KillPlayer(int photonPlayerID);
        void RequestUIShake();
    }
}