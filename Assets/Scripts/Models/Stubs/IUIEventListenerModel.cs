using System;
using System.Collections.Generic;
using UnityEngine;
using RR.Utility.Input;

namespace RR.Models.UIEventListenerModel
{
    public interface IUIEventListenerModel : IModel
    {
        event Action<Queue<RRUICommand>> UIEventRelay;
        void UIHasInputEventsToProcess(Queue<RRUICommand> events);
    }
}

