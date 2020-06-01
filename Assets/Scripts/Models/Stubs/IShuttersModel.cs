using System;

namespace RR.Models.ShuttersModel
{
    public enum ShutterState
    {
        Inactive,
        Open,
        Partial,
        Closed
    }
    
    public interface IShuttersModel : IModel
    {
        event Action<ShutterState, bool> StateChanged;
        event Action ShutterTransitionEnd;
        ShutterState State { get; }

        void SetState(ShutterState state, bool shake = true);
        void NotifyShutterTransitionEnd();
    }
}

