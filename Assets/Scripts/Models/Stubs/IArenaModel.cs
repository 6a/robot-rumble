using System;
using System.Collections;
using RR.Models.NetworkModel;
using RR.Facilitators.Platform;
using UnityEngine;

namespace RR.Models.ArenaModel
{
    public struct Pedestals
    {
        public IPedestal this[int i]
        {
            get { return _data[i]; }
        }

        private IPedestal[] _data;

        public Pedestals(IPedestal[] pdata)
        {
            _data = pdata;
        }
    }

    public interface IArenaModel : IModel
    {
        event Action<PlayerDetails> GameStart;
        event Action<Pedestals> PedestalsConfigured;
        event Action Reset;
        event Action<int> NewSessionSeed;
        event Action<PlayerDetails> PlayerCameraTransformUpdated;
        event Action EnablePostGameCamera;

        int Rings { get; }
        float Radius { get; }

        void SetRings(int rings);
        void SetRadius(float radius);
        void AlertPedestalsConfigured(Pedestals pedestalData);
        void AlertGenerationComplete();
        void AlertCameraSwitchReady();
        void PrepareForNewGame();
    }
}

