using System;
using RR.Models.NetworkModel;

namespace RR.Models.PostGameUIModel
{
    public interface IPostGameUIModel : IModel
    {
        event Action<bool, PlayerStatus> ActiveChanged;

        void SetActive(bool active);
        void ReturnClicked();
    }
}

