using UnityEngine;
using UnityEngine.UI;
using RR.Facilitators.UI;

namespace RR.Properties
{
    public class LeaderboardsUIProperties : BaseProperties
    {
        public GameObject Wrapper;
        public GameObject LoadingOverlay;
        public Button CloseButton;
        public Button RefreshButton;
        public Button RetryButton;
        public Image LoadingSpinner;
        public Text LoadingText;

        public Text ProfileNameText;
        public Text RankText;
        public Text WinsText;
        public Text DrawsText;
        public Text LossesText;
        public Text RatioText;
        public Text PlayedText;

        public GameObject LeaderboardsContent;
        public LeaderboardItem LeaderboardItemPrefab;
    }
}
