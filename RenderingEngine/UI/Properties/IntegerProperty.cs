using RenderingEngine.Datatypes;

namespace RenderingEngine.UI.Properties
{
    public class IntegerProperty : ObservableData
    {
        long _lower, _upper, _snap;
        private long _value;
        public long Value { get => _value; set => SetValue(value); }
        public long Lower {
            get { return _lower; }
            set {
                _lower = value;
                SetValue(_value);
            }
        }

        public long Upper {
            get { return _upper; }
            set {
                _upper = value;
                SetValue(_value);
            }
        }

        public long Snap {
            get { return _snap; }
            set {
                _snap = value;
                SetValue(_value);
            }
        }

        public void SetValue(long num)
        {
            if (num < _lower)
                num = _lower;

            if (num > _upper)
                num = _upper;

            _value = num;

            if (_snap > 1)
            {
                _value = _lower + ((_value - _lower) / _snap) * _snap;
            }

            DataChanged();
        }

        public IntegerProperty()
            : this(long.MinValue, long.MaxValue, 0, 0)
        {
        }

        public IntegerProperty(long lower, long upper, long snap, long value)
        {
            _lower = lower;
            _upper = upper;
            _snap = snap;
            Value = value;
        }
    }
}
