using System;
using UnityEngine;
using RR.Models.OptionsUIModel;

namespace RR.Models.GamestateModel.Impl
{
    public class GamestateModel : Model, IGamestateModel
    {
        public event Action<Gamestate> StateChanged = delegate { };

        public Gamestate State { get; private set; }

        public void Initialize()
        {
            State = Gamestate.Initializing;
        }

        public void SetState(Gamestate state)
        {
            if (State != state) 
            {
                State = state;
                EventDispatcher.Broadcast(StateChanged, State);

                if (State == Gamestate.Game && !Models.Get<IOptionsUIModel>().IsOpen)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
