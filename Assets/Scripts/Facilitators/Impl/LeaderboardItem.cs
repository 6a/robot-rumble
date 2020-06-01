using UnityEngine;
using UnityEngine.UI;

namespace RR.Facilitators.UI
{
    public class LeaderboardItem : MonoBehaviour, ILeaderboardItem
    {
        [SerializeField] private Text[] _fields = null;
        [SerializeField] private Image _background = null;
        [SerializeField] private Color _highlight = Color.white;
        [SerializeField] private Color _normal = Color.white;

        public void SetValues(int rank, string name, int wins, int draws, int losses, float ratio, int played)
        {
            _fields[0].text = rank.ToString();
            _fields[1].text = name;
            _fields[2].text = wins.ToString();
            _fields[3].text = draws.ToString();
            _fields[4].text = losses.ToString();
            _fields[5].text = ratio.ToString("0.00");
            _fields[6].text = played.ToString();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Highlight(bool highlight)
        {
            Color bgcolor = highlight ? _highlight : _normal;
            Color textcolor = highlight ? _normal : _highlight;

            _background.color = bgcolor;
            
            foreach (var textfield in _fields)
            {
                textfield.color = textcolor;
            }
        }
    }
}