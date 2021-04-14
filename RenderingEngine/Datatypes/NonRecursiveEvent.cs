using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Datatypes
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
}
