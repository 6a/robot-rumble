using System;
using UnityEngine;
using System.Collections.Generic;
using RR.Models.UIEventListenerModel;
using RR.Utility.Input;
using RR.Models.GamestateModel;
using RR.Models.PlayerCameraModel;

namespace RR.Models.OptionsUIModel.Impl
{
    public class OptionsUIModel : Model, IOptionsUIModel
    {
        public event Action<bool> ActiveChanged = delegate { };

        public bool CanOpenExitMenu { get; set; }
        public bool IsOpen { get { return _active; } }
        
        private bool _active;

        public void Initialize()
        {   
            CanOpenExitMenu = true;
            _active = false;
            Models.Get<IUIEventListenerModel>().UIEventRelay += OnUIEvent;

        }

        private void SetActive(bool active)
        {
            EventDispatcher.Broadcast(ActiveChanged, active);
            _active = active;
        }

        private void OnUIEvent(Queue<RRUICommand> inputs)
        {
            if (!_active && CanOpenExitMenu && inputs.Contains(RRUICommand.Exit))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SetActive(true);
            }
            else if (_active)
            {
                SetActive(false);
                if (Models.Get<IGamestateModel>().State == Gamestate.Game)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        public void Quit()
        {
            Application.Quit(0x0);
        }

        public void Return()
        {
            SetActive(false);
        }

        public void InvertYAxisChanged(bool invert)
        {
            PlayerPrefs.SetInt("invert-y", invert ? 1 : 0);
            Models.Get<IPlayerCameraModel>().SetInvertYAxis(invert);
        }
    }
}
