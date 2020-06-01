using System;
using UnityEngine;
using System.Collections.Generic;
using RR.Models.DBModel;
using RR.Models.LoginUIModel;
using System.Text;

namespace RR.Models.LeaderboardsUIModel.Impl
{
    public class LeaderboardsUIModel : Model, ILeaderboardsUIModel
    {
        public event Action<bool> ActiveChanged = delegate { };
        public event Action UpdateStarted = delegate { };
        public event Action<Leaderboards> UpdateSucceeded = delegate { };
        public event Action<string> UpdateFailed = delegate { };

        public bool CanOpenExitMenu { get; set; }
        
        private bool _active;

        public void Initialize()
        {   
            Models.Get<IDBModel>().LeaderboardsResult += OnLeaderboardsResult;

            UpdateLeaderboards();
        }

        private void UpdateLeaderboards()
        {
            var currentAuthUser = Models.Get<ILoginUIModel>().AuthenticatedUser;
            if (currentAuthUser.name != null)
            {
                Models.Get<IDBModel>().GetLeaderboards(Models.Get<ILoginUIModel>().AuthenticatedUser);
                EventDispatcher.Broadcast(UpdateStarted);
            }
            else
            {
                EventDispatcher.Broadcast(UpdateStarted);
                EventDispatcher.Broadcast(UpdateFailed, "You are not logged in");
            }
        }

        private void OnLeaderboardsResult(Response response)
        {
            if (response.Code == 200)
            {
                var leaderboard = JsonUtility.FromJson<Leaderboards>(response.Message);
                EventDispatcher.Broadcast(UpdateSucceeded, leaderboard);
            }
            else
            {
                Debug.Log($"Leaderboard fetch failed: {response.Message}");
                EventDispatcher.Broadcast(UpdateFailed, "Failed to update leaderboards");
            }
        }

        public void Fetch()
        {
            UpdateLeaderboards();
        }
        
        public void SetActive(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active);
            _active = active;

            if (_active) UpdateLeaderboards();
        }
    }
}
