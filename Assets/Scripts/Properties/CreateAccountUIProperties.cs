using UnityEngine;
using UnityEngine.UI;
using RR.Facilitators.Input.Impl;

namespace RR.Properties
{
    public class CreateAccountUIProperties : BaseProperties
    {
        public Button Create;
        public Text Cancel;
        public Text ErrorMessage;
        public InputField Username;
        public InputField Password;
        public Image Spinner;
    }
}
