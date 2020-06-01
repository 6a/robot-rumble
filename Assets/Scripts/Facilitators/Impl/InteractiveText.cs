using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace RR.Facilitators.Input.Impl
{
    public class InteractiveText : MonoBehaviour, IInteractiveText
    {
        public event Action Click = delegate { };
        
        [SerializeField] private Button _parent = null;
        [SerializeField] private Color _normal = Color.white;
        [SerializeField] private Color _selected = Color.white;
        [SerializeField] private Color _hovered = Color.white;

        private Color _mostRecentColor;

        private void Awake() 
        {
            var trigger = GetComponentInParent<EventTrigger>();
            EventTrigger.Entry onclick = new EventTrigger.Entry();
            onclick.eventID = EventTriggerType.PointerClick;
            onclick.callback.AddListener((data) => 
            { 
                var pd = (PointerEventData)data; 
                if (pd.button == PointerEventData.InputButton.Left) 
                {
                    OnClick(); 
                }
            });

            EventTrigger.Entry onenter = new EventTrigger.Entry();
            onenter.eventID = EventTriggerType.PointerEnter;
            onenter.callback.AddListener((data) => OnEnter());
            
            EventTrigger.Entry onexit = new EventTrigger.Entry();
            onexit.eventID = EventTriggerType.PointerExit;
            onexit.callback.AddListener((data) => OnExit());    

            EventTrigger.Entry onselect = new EventTrigger.Entry();
            onselect.eventID = EventTriggerType.Select;
            onselect.callback.AddListener((data) => OnSelect());    

            EventTrigger.Entry ondeselect = new EventTrigger.Entry();
            ondeselect.eventID = EventTriggerType.Deselect;
            ondeselect.callback.AddListener((data) => OnDeselect());    

            trigger.triggers.Add(onclick);
            trigger.triggers.Add(onenter);
            trigger.triggers.Add(onexit);
            trigger.triggers.Add(onselect);
            trigger.triggers.Add(ondeselect);

            _parent.onClick.AddListener(OnClick);

            _mostRecentColor = _normal;
        }

        private void OnClick()
        {
            if (Click != null)
            {
                Click();
                GetComponent<Text>().color = _mostRecentColor;
            }
        }
        
        private void OnEnter()
        {
            GetComponent<Text>().color = _mostRecentColor = _hovered;
        }

        private void OnExit()
        {
            GetComponent<Text>().color = _mostRecentColor = _normal;
        }

        private void OnSelect()
        {
            GetComponent<Text>().color = _selected;
        }

        private void OnDeselect()
        {
            GetComponent<Text>().color = _mostRecentColor;
        }
    }
}