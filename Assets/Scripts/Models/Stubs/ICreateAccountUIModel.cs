using System;
using System.Collections.Generic;
using RR.Utility.Input;
using RR.Models.DBModel;

namespace RR.Models.CreateAccountUIModel
{
    public interface ICreateAccountUIModel : IModel
    {
        bool Interactable { get; }
        event Action<bool> InteractableChanged;
        event Action<Queue<RRUICommand>> UIInputEvent;
        event Action<Response> CreateAccountCompleted;
        event Action ClearForm;

        void UserClickedCancel();
        void UserClickedCreate(User credentials);
    }
}

