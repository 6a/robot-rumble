using System;

namespace RR.Utility
{
    public class EventDispatcher
    {
        public void Broadcast(Action action)
        {
            if (action != null) action();
        }

        public void Broadcast<T>(Action<T> action, T value)
        {
            if (action != null) action(value);
        }

        public void Broadcast<T1, T2>(Action<T1, T2> action, T1 value1, T2 value2)
        {
            if (action != null) action(value1, value2);
        }
    }
}