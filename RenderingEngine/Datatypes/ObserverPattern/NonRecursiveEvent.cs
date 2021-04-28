using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Datatypes.ObserverPattern
{
    public class NonRecursiveEvent
    {
        public event Action Event;
        bool _invoking = false;

        public void Invoke()
        {
            if (_invoking)
                return;

            _invoking = true;
            Event?.Invoke();
            _invoking = false;
        }
    }

    public class NonRecursiveEvent<T>
    {
        public event Action<T> Event;
        bool _invoking = false;

        public void Invoke(T arg)
        {
            if (_invoking)
                return;

            _invoking = true;
            Event?.Invoke(arg);
            _invoking = false;
        }

        public void RemoveCallbacks()
        {
            Event = null;
        }

        public void AddCallback(object callback)
        {
            Event += (Action<T>)callback;
        }

        public void RemoveCallback(object callback)
        {
            Event -= (Action<T>)callback;
        }
    }
}
