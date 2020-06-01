using UnityEngine;

namespace RR.Facilitators.UI
{
    public interface IPlayerScoreRow
    {
        void SetName(string name);
        void SetAsLocalPlayerRow();
        void SetAsOpponentRow();
        void SetAsDisconnected();
        void UpdateValues(int health, int position);
        void SetYOffset(float offset);
    }
}