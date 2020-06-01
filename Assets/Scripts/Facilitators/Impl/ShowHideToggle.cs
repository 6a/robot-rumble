using UnityEngine;
using UnityEngine.UI;

namespace RR.Facilitators.UI.Impl
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Canvas))]
    public class ShowHideToggle : MonoBehaviour, IShowHideToggle
    {
        public InputField Target;
        public Image ShowIcon;
        public Image HideIcon;

        private bool _show;

        private void Awake()
        {
            _show = true;
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _show = !_show;

            Target.contentType = _show ? InputField.ContentType.Password : InputField.ContentType.Standard;
            Target.gameObject.SetActive(false); Target.gameObject.SetActive(true);
            ShowIcon.gameObject.SetActive(_show);
            HideIcon.gameObject.SetActive(!_show);
        }

        public void Reset()
        {
            _show = true;
            OnClick();
        }
    }
}
