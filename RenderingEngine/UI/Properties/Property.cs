using RenderingEngine.Datatypes.ObserverPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Properties
{
    public abstract class Property<T> : ObservableData<T>
    {
        protected T _value;
        public T Value { 
            get => _value; 
            set {
                SetValue(value);
                DataChanged(_value);
            }
        }

        protected virtual void SetValue(T val)
        {
            _value = val;
        }

        public Property(T value)
        {
            _value = value;
        }
    }
}
