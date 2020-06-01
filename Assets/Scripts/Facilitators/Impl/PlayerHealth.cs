using UnityEngine;
using UnityEngine.UI;

namespace RR.Facilitators.UI
{
    public class PlayerHealth : MonoBehaviour 
    {
        [SerializeField] private Slider _healthbar = null;
        [SerializeField] private Image _pulse = null;

        private void FixedUpdate() 
        {
            if (PhotonNetwork.player != null)
            {
                _healthbar.value = Mathf.Clamp(PhotonNetwork.player.GetScore(), _healthbar.minValue, _healthbar.maxValue);
                _pulse.gameObject.SetActive(_healthbar.value <= 20);
            }
        }
    }
}