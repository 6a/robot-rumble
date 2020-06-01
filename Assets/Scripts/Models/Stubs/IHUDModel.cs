using System;

namespace RR.Models.HUDModel
{
    public interface IHUDModel : IModel
    {
        event Action<bool> ActiveChanged;
        event Action Reset;

        void SetActive(bool active);
    }
}

