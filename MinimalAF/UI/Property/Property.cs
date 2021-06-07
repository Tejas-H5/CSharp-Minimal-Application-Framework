using MinimalAF.Datatypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI
{
    public abstract class Property<T> : ObservableData<T>, IProperty
    {
        protected T _value;
        public T Value {
            get => _value;
            set {
                _value = value;
                DataChanged(_value);
            }
        }

        public Type InnerType {
            get {
                return typeof(T);
            }
        }

        public Type PropertyType {
            get {
                return typeof(Property<T>);
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

        protected virtual void SetValueInternal(T val)
        {
            _value = val;
        }

        public abstract Property<T> Copy();

        public void SetValue(object obj)
        {
            Value = (T)obj;
        }

        Dictionary<Action<object>, Action<T>> _actions = null;

        public void AddCallback(Action<object> a)
        {
            if(_actions == null)
                _actions = new Dictionary<Action<object>, Action<T>>();

            if (_actions.ContainsKey(a))
                return;

            Action<T> aT = new Action<T>(o => a(o));

            OnDataChanged += aT;
            _actions.Add(a, aT);
        }

        public void RemoveCallback(Action<object> a)
        {
            if (_actions == null)
                _actions = new Dictionary<Action<object>, Action<T>>();

            if (!_actions.ContainsKey(a))
                return;

            Action<T> aT = _actions[a];

            OnDataChanged -= aT;
            _actions.Remove(a);
        }
    }
}
