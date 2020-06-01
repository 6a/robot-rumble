using RR.Presenters;
using RR.Properties;
using RR.Models.DBModel;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RR.Facilitators.UI;

namespace RR.Views
{
    public class LeaderboardsUIView : BaseView<LeaderboardsUIPresenter, LeaderboardsUIProperties>
    {
        private const float MIN_UPDATE_DELAY = 0.5f;

        private Coroutine _tableGenerationCoroutine;
        private Coroutine _delayedUpdateCoroutine;
        private List<ILeaderboardItem> _leaderboardItems = new List<ILeaderboardItem>();
        private float _updateStartTime;

        public override void Initialize()
        {
            AddListeners();
            Presenter.Initialize();
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.ActiveChanged += OnActiveChanged;
            Presenter.UpdateStarted += OnUpdateStarted;
            Presenter.UpdateFailed += OnUpdateFailed;
            Presenter.UpdateSucceeded += OnUpdateSucceeded;

            Properties.CloseButton.onClick.AddListener(OnQuitClicked);
            Properties.RefreshButton.onClick.AddListener(OnReload);
            Properties.RetryButton.onClick.AddListener(OnReload);
        }

        private void RemoveListeners()
        {
            Presenter.ActiveChanged -= OnActiveChanged;
            Presenter.UpdateStarted -= OnUpdateStarted;
            Presenter.UpdateFailed -= OnUpdateFailed;
            Presenter.UpdateSucceeded -= OnUpdateSucceeded;

            Properties.CloseButton.onClick.RemoveListener(OnQuitClicked);
            Properties.RefreshButton.onClick.RemoveListener(OnReload);
            Properties.RetryButton.onClick.RemoveListener(OnReload);
        }

        private void OnQuitClicked()
        {
            Presenter.Close();
        }

        private void OnActiveChanged(bool active)
        {
            Properties.Wrapper.SetActive(active);
            if (_tableGenerationCoroutine != null) StopCoroutine(_tableGenerationCoroutine);
            if (!active) CleanupTable();
        }

        private void OnUpdateStarted()
        {
            Properties.LoadingOverlay.SetActive(true);
            Properties.LoadingText.text = "Loading";
            Properties.RetryButton.gameObject.SetActive(false);
            Properties.LoadingSpinner.gameObject.SetActive(true);
            _updateStartTime = Time.time;
        }

        private void OnUpdateFailed(string msg)
        {
            Properties.LoadingOverlay.SetActive(true);
            Properties.LoadingText.text = msg;
            Properties.RetryButton.gameObject.SetActive(true);
            Properties.LoadingSpinner.gameObject.SetActive(false);
        }

        private void OnUpdateSucceeded(Leaderboards leaderboards)
        {
            if (_delayedUpdateCoroutine != null) StopCoroutine(_delayedUpdateCoroutine);
            _delayedUpdateCoroutine = StartCoroutine(DelayedUpdate(leaderboards));
        }

        private void OnReload()
        {
            OnUpdateStarted();
            Presenter.Fetch();
        }

        private void FillProfile(LeaderboardRow playerRow)
        {
            Properties.ProfileNameText.text = playerRow.name;
            Properties.RankText.text = $"{playerRow.rank.ToString()} / {playerRow.outof.ToString()}";
            Properties.WinsText.text = playerRow.wins.ToString();
            Properties.DrawsText.text = playerRow.draws.ToString();
            Properties.LossesText.text = playerRow.losses.ToString();
            Properties.RatioText.text = playerRow.ratio.ToString("0.00");
            Properties.PlayedText.text = playerRow.played.ToString();
        }

        private IEnumerator DelayedUpdate(Leaderboards leaderboards)
        {
            yield return new WaitForSeconds(Mathf.Max(MIN_UPDATE_DELAY - (Time.time - _updateStartTime)));

            Properties.LoadingOverlay.SetActive(false);
            Properties.RetryButton.gameObject.SetActive(false);
            Properties.LoadingSpinner.gameObject.SetActive(false);

            CleanupTable();

            FillProfile(leaderboards.user);

            if (_tableGenerationCoroutine != null) StopCoroutine(_tableGenerationCoroutine);
            _tableGenerationCoroutine = StartCoroutine(FillTableAsync(leaderboards.leaderboard, leaderboards.user.name));
        }

        private IEnumerator FillTableAsync(LeaderboardRow[] rows, string localUser)
        {
            var total = rows.Length;
            var pooled = _leaderboardItems.Count;

            int placed = 0;
            foreach (var row in rows)
            {
                ILeaderboardItem item;
                if (placed < pooled)
                {
                    item = _leaderboardItems[placed];
                }
                else
                {
                    item = Instantiate(Properties.LeaderboardItemPrefab, Properties.LeaderboardsContent.transform) as ILeaderboardItem;
                    _leaderboardItems.Add(item);
                }

                item.SetActive(true);
                item.SetValues(row.rank, row.name, row.wins, row.draws, row.losses, row.ratio, row.played);
                item.Highlight(row.name == localUser);

                placed++;

                yield return new WaitForSeconds(0.02f);
            }
        }

        private void CleanupTable()
        {
            foreach (var row in _leaderboardItems)
            {
                row.SetActive(false);
            }
        }
    }
}
