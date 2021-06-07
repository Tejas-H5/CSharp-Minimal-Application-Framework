using System;

namespace MinimalAF.Datatypes
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
    }
}
