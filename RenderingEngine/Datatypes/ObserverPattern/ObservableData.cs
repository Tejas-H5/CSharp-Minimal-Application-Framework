using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Datatypes.ObserverPattern
{
    public class ObservableData<T>
    {
        public event Action<T> OnDataChanged {
            add {
                _onDataChanged.Event += value;
            }
            remove {
                _onDataChanged.Event -= value;
            }
        }

        bool _locked = false;
        private NonRecursiveEvent<T> _onDataChanged = new NonRecursiveEvent<T>();

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock(T args)
        {
            _locked = false;
            DataChanged(args);
        }

        protected void DataChanged(T args)
        {
            //Prevent circular invocation
            if (_locked)
                return;

            _onDataChanged?.Invoke(args);
        }
    }
}
