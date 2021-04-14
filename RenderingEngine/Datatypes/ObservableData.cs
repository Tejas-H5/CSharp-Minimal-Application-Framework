using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Datatypes
{
    public class ObservableData
    {
        public event Action OnDataChanged {
            add {
                _onDataChanged.Event += value;
            }
            remove {
                _onDataChanged.Event-= value;
            }
        }

        bool _locked = false;
        private NonRecursiveEvent _onDataChanged = new NonRecursiveEvent();

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
            DataChanged();
        }

        protected void DataChanged()
        {
            //Prevent circular invocation
            if (_locked)
                return;

            _onDataChanged?.Invoke();
        }
    }
}
