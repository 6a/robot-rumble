using UnityEngine;
using UnityEngine.UI;

namespace RR.Facilitators.UI
{
    public class PlayerScoreRow : MonoBehaviour, IPlayerScoreRow
    {
        [SerializeField] private Slider _healthbar = null;
        [SerializeField] private Text _nameText = null;
        [SerializeField] private Text _positionText = null;
        [SerializeField] private Image[] _recolorTargets = null;
        [SerializeField] private Color _localPlayerBorderColor = Color.white;
        [SerializeField] private Color _disconnectedBorderColor = Color.white;
        [SerializeField] private Color _normalColor = Color.white;

        public void SetName(string name)
        {
            _nameText.text = name;
        }

        public void SetAsLocalPlayerRow()
        {
            foreach (var target in _recolorTargets)
            {
                target.color = _localPlayerBorderColor;
            }
        }

        public void SetAsOpponentRow()
        {
            foreach (var target in _recolorTargets)
            {
                target.color = _normalColor;
            }
        }

        public void SetAsDisconnected()
        {
            foreach (var target in _recolorTargets)
            {
                target.color = _disconnectedBorderColor;
            }
        }

        public void UpdateValues(int health, int position)
        {
            _healthbar.value = Mathf.Clamp(health, _healthbar.minValue, _healthbar.maxValue);
            
            var positionText = position == 0 ? "x" : position.ToString();
            _positionText.text = positionText;
        }

        public void SetYOffset(float offset)
        {
            var pos = GetComponent<RectTransform>().anchoredPosition;
            pos.y = offset;
            GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }
}