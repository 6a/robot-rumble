using System;
using RR.Models.DBModel;

namespace RR.Models.LeaderboardsUIModel
{
    public interface ILeaderboardsUIModel : IModel
    {
        event Action<bool> ActiveChanged;
        event Action UpdateStarted;
        event Action<Leaderboards> UpdateSucceeded;
        event Action<string> UpdateFailed;

        void SetActive(bool active);
        void Fetch();
    }
}

