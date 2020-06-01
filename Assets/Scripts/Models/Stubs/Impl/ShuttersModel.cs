using System;
using UnityEngine;
using RR.Models.GamestateModel;

namespace RR.Models.ShuttersModel.Impl
{
    public class ShuttersModel : Model, IShuttersModel
    {
        public event Action<ShutterState, bool> StateChanged = delegate { };
        public event Action ShutterTransitionEnd = delegate { };

        public ShutterState State { get; private set; }

        public void Initialize()
        {

        }
        
        public void SetState(ShutterState state, bool shake = true)
        {
            if (state != State) 
            {
                State = state;

                EventDispatcher.Broadcast(StateChanged, State, shake);
            }
        }

        public void NotifyShutterTransitionEnd()
        {
            EventDispatcher.Broadcast(ShutterTransitionEnd);
        }
    }
}
