using UnityEngine;
using UnityEngine.UI;

namespace RR.Facilitators.UI
{
    public class FloatingHealthBar : MonoBehaviour, IFloatingHealthBar
    {
        [SerializeField] private Text _name = null;
        [SerializeField] private Slider _health = null;

        private Transform _anchor;
        private PhotonView _photonView;

        private bool _active;
        private void FixedUpdate() 
        {
            if (!_active) return;

            if (_photonView && _photonView.owner != null)
            {
                HandleHealthbar();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void HandleHealthbar()
        {
            if (Camera.main != null)
            {
                var forward = Camera.main.transform.forward;
                var heading = _anchor.transform.position - Camera.main.transform.position;
                var dot = Vector3.Dot(heading, forward);

                if (dot > 0)
                {
                    var health = _photonView.owner.GetScore();
                    _health.value = Mathf.RoundToInt(Mathf.Clamp(health, _health.minValue, _health.maxValue));

                    var worldPos = _anchor.transform.position;
                    var screenPos = Camera.main.WorldToScreenPoint(worldPos);
                    screenPos.z = 0;
                    GetComponent<RectTransform>().position = screenPos;
                }
            }
            else
            {
                GetComponent<RectTransform>().position = new Vector3(-1000, -1000, -1000);
            }
        }

        public void Init(PhotonView photonView, Transform anchor)
        {
            _anchor = anchor;
            _name.text = photonView.owner.NickName;
            _health.value = Mathf.RoundToInt(Mathf.Clamp(photonView.owner.GetScore(), _health.minValue, _health.maxValue));
            _photonView = photonView;
            _active = true;

            GetComponent<RectTransform>().position = new Vector3(-1000, -1000, -1000);
        }

        public void Destroy()
        {
            _active = false;
            Destroy(gameObject);
        }
    }
}