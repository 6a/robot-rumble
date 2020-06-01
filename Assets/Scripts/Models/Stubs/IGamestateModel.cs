using System;
using RR.Models.NetPlayerModel;

namespace RR.Models.GamestateModel
{
    public enum Gamestate
    {
        Initializing,
        MainMenu,
        Lobby,
        Matchmaking,
        ReadyCheck,
        ReadyCheckFailed,
        StartingGame,
        Game,
        Postgame,
        NetworkError,
    }

    public interface IGamestateModel : IModel
    {
        event Action<Gamestate> StateChanged;
        
        Gamestate State { get; }
        void SetState(Gamestate state);
    }
}

