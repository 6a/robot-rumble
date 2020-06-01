using System;

namespace RR.Models.OptionsUIModel
{
    public interface IOptionsUIModel : IModel
    {
        event Action<bool> ActiveChanged;

        bool IsOpen { get; }
        bool CanOpenExitMenu { get; set; }

        void Quit();
        void Return();
        void InvertYAxisChanged(bool invert);
    }
}

