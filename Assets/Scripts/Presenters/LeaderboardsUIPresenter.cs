using System;
using RR.Models.LeaderboardsUIModel;
using RR.Models.DBModel;

namespace RR.Presenters
{
    public class LeaderboardsUIPresenter : BasePresenter
    {
        public event Action<bool> ActiveChanged = delegate { };
        public event Action UpdateStarted = delegate { };
        public event Action<Leaderboards> UpdateSucceeded = delegate { };
        public event Action<string> UpdateFailed = delegate { };

        public override void Initialize()
        {
            Models.Get<ILeaderboardsUIModel>().ActiveChanged += OnActiveChanged;
            Models.Get<ILeaderboardsUIModel>().UpdateStarted += OnUpdateStarted;
            Models.Get<ILeaderboardsUIModel>().UpdateFailed += OnUpdateFailed;
            Models.Get<ILeaderboardsUIModel>().UpdateSucceeded += OnUpdateSucceeded;
        }
        
        public override void Dispose()
        {
            Models.Get<ILeaderboardsUIModel>().ActiveChanged -= OnActiveChanged;
            Models.Get<ILeaderboardsUIModel>().UpdateStarted -= OnUpdateStarted;
            Models.Get<ILeaderboardsUIModel>().UpdateFailed -= OnUpdateFailed;
            Models.Get<ILeaderboardsUIModel>().UpdateSucceeded -= OnUpdateSucceeded;
        }

        private void OnActiveChanged(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active);
        }

        private void OnUpdateStarted()
        {
            EventDispatcher.Broadcast(UpdateStarted);
        }

        private void OnUpdateFailed(string msg)
        {
            EventDispatcher.Broadcast(UpdateFailed, msg);
        }

        private void OnUpdateSucceeded(Leaderboards leaderboards)
        {
            EventDispatcher.Broadcast(UpdateSucceeded, leaderboards);
        }

        public void Fetch()
        {
            Models.Get<ILeaderboardsUIModel>().Fetch();
        }

        public void Close()
        {
            Models.Get<ILeaderboardsUIModel>().SetActive(false);
        }
    }
}
