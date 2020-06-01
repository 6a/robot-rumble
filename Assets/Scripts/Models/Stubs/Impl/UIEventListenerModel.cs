using System;
using System.Collections.Generic;
using RR.Utility.Input;
using UnityEngine;

namespace RR.Models.UIEventListenerModel.Impl
{
    public class UIEventListenerModel : Model, IUIEventListenerModel
    {
        public event Action<Queue<RRUICommand>> UIEventRelay = delegate { };

        public void Initialize()
        {
            
        }
        
        public void UIHasInputEventsToProcess(Queue<RRUICommand> events)
        {
            EventDispatcher.Broadcast(UIEventRelay, events);
        }
    }
}
