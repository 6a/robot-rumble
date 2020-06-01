using System;
using UnityEngine;
using System.Collections.Generic;
using RR.Models.GamestateModel;

namespace RR.Models.HUDModel.Impl
{
    public class HUDModel : Model, IHUDModel
    {
        public event Action<bool> ActiveChanged = delegate { };
        public event Action Reset = delegate { };

        public void Initialize()
        {   
            Models.Get<IGamestateModel>().StateChanged += OnGamestateChanged;
        }

        public void SetActive(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active);
        }

        private void OnGamestateChanged(Gamestate state)
        {
            var active = state == Gamestate.StartingGame || state == Gamestate.Game;
            SetActive(active);

            if (state == Gamestate.Lobby)
            {
                EventDispatcher.Broadcast(Reset);
            }
        }
    }
}
