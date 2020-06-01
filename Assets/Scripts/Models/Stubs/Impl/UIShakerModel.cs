using System;
using UnityEngine;

namespace RR.Models.UIShakerModel.Impl
{
    public class UIShakerModel : Model, IUIShakerModel
    {
        public event Action ShakeStart = delegate { };
        public event Action ShakeEnd = delegate { };

        public void Initialize()
        {

        }

        public void Shake()
        {
            EventDispatcher.Broadcast(ShakeStart);
        }

        public void NotifyShakeEnd()
        {
            EventDispatcher.Broadcast(ShakeEnd);
        }
    }
}
