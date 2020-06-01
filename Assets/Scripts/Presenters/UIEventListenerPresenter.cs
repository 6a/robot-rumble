using System;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.UIEventListenerModel;

namespace RR.Presenters
{
    public class UIEventListenerPresenter : BasePresenter
    {
        public override void Initialize()
        {

        }
        
        public override void Dispose()
        {

        }
        
        // Notifiers
        public void UIHasInputEventsToProcess(Queue<RRUICommand> events)
        {
            Models.Get<IUIEventListenerModel>().UIHasInputEventsToProcess(events);
        }

        // Event listeners
    }
}
