using System;

namespace MinimalAF.UI
{
    public class FloatProperty : Property<float>
    {
        float _lower, _upper, _snap;

        public float Lower {
            get { return _lower; }
            set {
                _lower = value;

                Value = _value;
            }
        }

        public float Upper {
            get { return _upper; }
            set {
                _upper = value;

                Value = _value;
            }
        }

        public float Snap {
            get { return _snap; }
            set {
                _snap = value;
                Value = _value;
            }
        }

        protected override void SetValueInternal(float num)
        {
            if (num < _lower)
                num = _lower;

            if (num > _upper)
                num = _upper;


            if (_snap > 0.0)
            {
                num = _lower + MathF.Round((num - _lower) / _snap) * _snap;
            }

            //if (num < _lower)
            //                num = _lower;

            if (num > _upper)
                num = _upper;

            _value = num;
        }

        public override Property<float> Copy()
        {
            return new FloatProperty(_value, _lower, _upper, _snap);
        }

        public FloatProperty()
            : this(0, float.MinValue, float.MaxValue, -1)
        {
        }

        public FloatProperty(float value, float lower, float upper, float snap)
            : base(value)
        {
            _lower = lower;
            _upper = upper;
            _snap = snap;
            Value = value;
        }
    }
}
