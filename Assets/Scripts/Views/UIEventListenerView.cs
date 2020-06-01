using RR.Presenters;
using RR.Properties;
using RR.Utility.Input;
using UnityEngine;
using System.Collections.Generic;

namespace RR.Views
{
    public class UIEventListenerView : BaseView<UIEventListenerPresenter, UIEventListenerProperties>
    {
        private Queue<RRUICommand> _pendingCommands;

        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();

            _pendingCommands = new Queue<RRUICommand>();
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 

        }

        private void RemoveListeners()
        {
        }

        private void Update() 
        {
            CaptureKeyboardEvents();
            Propagate();
        }

        private void CaptureKeyboardEvents()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) _pendingCommands.Enqueue(RRUICommand.Exit);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) _pendingCommands.Enqueue(RRUICommand.Submit);

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab)) _pendingCommands.Enqueue(RRUICommand.NavigateBack);
            else if (Input.GetKeyDown(KeyCode.Tab)) _pendingCommands.Enqueue(RRUICommand.NavigateForward);
        }

        private void Propagate()
        {
            if (_pendingCommands.Count > 0)
            {
                Presenter.UIHasInputEventsToProcess(_pendingCommands);
                _pendingCommands.Clear();
            }
        }
    }
}
