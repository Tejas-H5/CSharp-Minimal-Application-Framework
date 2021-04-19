using RenderingEngine.Datatypes;

namespace RenderingEngine.UI.Property
{
    public class IntegerProperty : Property<int>
    {
        int _lower, _upper, _snap;
        public int Lower {
            get { return _lower; }
            set {
                _lower = value;
                Value = _value;
            }
        }

        public int Upper {
            get { return _upper; }
            set {
                _upper = value;
                Value = _value;
            }
        }

        public int Snap {
            get { return _snap; }
            set {
                _snap = value;
                Value = _value;
            }
        }

        protected override void SetValue(int num)
        {
            if (num < _lower)
                num = _lower;

            if (num > _upper)
                num = _upper;

            _value = num;

            if (_snap > 1)
            {
                _value = _lower + (_value - _lower) / _snap * _snap;
            }
        }

        public IntegerProperty()
            : this(int.MinValue, int.MaxValue, 0, 0)
        {
        }

        public IntegerProperty(int lower, int upper, int snap, int value)
            : base(value)
        {
            _lower = lower;
            _upper = upper;
            _snap = snap;
            Value = value;
        }
    }
}
