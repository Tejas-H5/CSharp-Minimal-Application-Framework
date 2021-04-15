using RenderingEngine.Datatypes;

namespace RenderingEngine.UI.Properties
{
    public class IntegerProperty : Property<long>
    {
        long _lower, _upper, _snap;
        public long Lower {
            get { return _lower; }
            set {
                _lower = value;
                Value = _value;
            }
        }

        public long Upper {
            get { return _upper; }
            set {
                _upper = value;
                Value = _value;
            }
        }

        public long Snap {
            get { return _snap; }
            set {
                _snap = value;
                Value = _value;
            }
        }

        protected override void SetValue(long num)
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
        }

        public IntegerProperty()
            : this(long.MinValue, long.MaxValue, 0, 0)
        {
        }

        public IntegerProperty(long lower, long upper, long snap, long value)
            : base(value)
        {
            _lower = lower;
            _upper = upper;
            _snap = snap;
            Value = value;
        }
    }
}
