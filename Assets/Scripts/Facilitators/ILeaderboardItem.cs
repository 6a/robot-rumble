

namespace RR.Facilitators.UI
{
    public interface ILeaderboardItem
    {
        void SetValues(int rank, string name, int wins, int draws, int losses, float ratio, int played);
        void SetActive(bool active);
        void Highlight(bool highlight);
    }
}