﻿using System;
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

        public void AddCallback(object callback)
        {
            _onDataChanged.AddCallback(callback);
        }

        public void RemoveCallback(object callback)
        {
            _onDataChanged.RemoveCallback(callback);
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
