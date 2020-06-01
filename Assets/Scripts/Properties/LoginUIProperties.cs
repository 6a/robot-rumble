using UnityEngine;
using UnityEngine.UI;
using RR.Facilitators.Input.Impl;

namespace RR.Properties
{
    public class LoginUIProperties : BaseProperties
    {
        public Button Login;
        public Text CreateAccount;
        public Text ErrorMessage;
        public InputField Username;
        public InputField Password;
        public Image Spinner;
    }
}
