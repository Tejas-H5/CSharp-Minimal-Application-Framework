using RenderingEngine.Datatypes.ObserverPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Property
{
    public abstract class Property<T> : ObservableData<T>, IProperty
    {
        protected T _value;
        public T Value {
            get => _value;
            set {
                SetValue(value);
                DataChanged(_value);
            }
        }

        public Type PropertyType {
            get {
                return typeof(T);
            }
        }

        public Property(T value, Action<T> onChangedCallback)
        {
            _value = value;
            if(onChangedCallback!=null)
                OnDataChanged += onChangedCallback;
        }

        public Property(T value)
            : this(value, null)
        {
        }

        protected virtual void SetValue(T val)
        {
            _value = val;
        }
    }
}
