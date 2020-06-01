using UnityEngine;
using UnityEngine.UI;

namespace RR.Facilitators.UI
{
    public class Countdown : MonoBehaviour 
    {
        [SerializeField] private Image _border = null;
        [SerializeField] private Text _text = null;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _panicColor = Color.white;

        private bool _running = false;
        private double _startTime = 0;

        private void FixedUpdate() 
        {
            if (!_running)
            {
                _text.text = GetFormattedTime(60);

                if (PhotonNetwork.inRoom)
                {
                    if (GetStartTime())
                    {
                        _running = true; 
                    }
                }
            }
            else
            {
                var time = System.Math.Max(60 - (PhotonNetwork.time - _startTime), 0);
                _text.text = GetFormattedTime(time);
                if (time <= 10)
                {
                    _text.color = _panicColor;
                    _border.color = _panicColor;
                }
                else
                {
                    _text.color = _defaultColor;
                    _border.color = _defaultColor;
                }
            }
        }

        private string GetFormattedTime(double time)
        {
            var output = "";
            if (time > 10)
            {
                output = System.Math.Floor(time).ToString();
            }
            else
            {
                output = time.ToString("0.0");
            }

            return output;
        }

        private bool GetStartTime()
        {
            bool found = false;
            if (PhotonNetwork.inRoom)
            {
                object customProperty = null;
                double startTime = 0;
                found = PhotonNetwork.room.CustomProperties.TryGetValue("start-time", out customProperty);

                if (found)
                {
                    startTime = (double)customProperty;
                    _startTime = startTime;
                }
            }

            return found;
        }

        public void Reset()
        {
            _running = false;
            _text.color = _defaultColor;
            _border.color = _defaultColor;
        }
    }
}