using System;
using UnityEngine;
using System.Collections.Generic;
using RR.Models.GamestateModel;
using RR.Models.NetworkModel;
using RR.Models.ShuttersModel;
using RR.Models.ArenaModel;
using RR.Models.DBModel;

namespace RR.Models.PostGameUIModel.Impl
{
    public class PostGameUIModel : Model, IPostGameUIModel
    {
        public event Action<bool, PlayerStatus> ActiveChanged = delegate { };

        private bool _waitingForShutterTransition;

        public void Initialize()
        {   
            Models.Get<IGamestateModel>().StateChanged += OnGamestateChanged;
            Models.Get<IShuttersModel>().ShutterTransitionEnd += OnShutterTransitionEnd;
        }

        public void SetActive(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active, Models.Get<INetworkModel>().LocalPlayerStatus);
        }

        public void ReturnClicked()
        {
            _waitingForShutterTransition = true;
            Models.Get<IGamestateModel>().SetState(Gamestate.Lobby);
            Models.Get<IShuttersModel>().SetState(ShutterState.Closed);
        }

        private void OnGamestateChanged(Gamestate state)
        {
            SetActive(state == Gamestate.Postgame);
        }

        private void OnShutterTransitionEnd()
        {
            if (_waitingForShutterTransition)
            {
                _waitingForShutterTransition = false;

                SetActive(false);

                PhotonNetwork.Disconnect();
            }
        }
    }
}
