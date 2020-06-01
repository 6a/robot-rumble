using UnityEngine;
using UnityEngine.UI;

namespace RR.Properties
{
    public class PostGameUIProperties : BaseProperties
    {
        public GameObject Wrapper;
        public GameObject DeathTextWrapper;
        public Text DeathText;
        public Text NonDeathGameEndText;
        public Button ReturnButton;

        public Color DeathColor;
        public Color ButtonStandardColor;
    }
}
