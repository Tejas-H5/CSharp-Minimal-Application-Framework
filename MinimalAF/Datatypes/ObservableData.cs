using System;

namespace MinimalAF.Datatypes
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

        public void RemoveCallbacks()
        {
            _onDataChanged.RemoveCallbacks();
        }

        public void Lock()
        {
            _locked = true;
        }

        public void UnlockNonInvoking()
        {
            _locked = false;
        }

        public void Unlock(T obj)
        {
            _locked = false;
            _onDataChanged.Invoke(obj);
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
