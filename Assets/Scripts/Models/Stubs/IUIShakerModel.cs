using System;

namespace RR.Models.UIShakerModel
{
    public interface IUIShakerModel : IModel
    {
        event Action ShakeStart;
        event Action ShakeEnd;
        void Shake();
        void NotifyShakeEnd();
    }
}

