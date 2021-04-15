using RenderingEngine.Datatypes;
using RenderingEngine.Util;
using System;

namespace RenderingEngine.UI.Properties
{
    public class FloatProperty : Property<double>
    {
        double _lower, _upper, _snap;

        public double Lower {
            get { return _lower; }
            set {
                _lower = value;

                Value = _value;
            }
        }

        public double Upper {
            get { return _upper; }
            set {
                _upper = value;

                Value = _value;
            }
        }

        public double Snap {
            get { return _snap; }
            set {
                _snap = value;
                Value = _value;
            }
        }

        protected override void SetValue(double num)
        {
            if (num < _lower)
                num = _lower;

            if (num > _upper)
                num = _upper;


            if (_snap > 0.0)
            {
                num = _lower + Math.Round((num - _lower)/_snap) * _snap;
            }

            //if (num < _lower)
//                num = _lower;

            if (num > _upper)
                num = _upper;

            _value = num;
        }

        public FloatProperty()
            : this(double.MinValue, double.MaxValue, -1, 0)
        {
        }

        public FloatProperty(double lower, double upper, double snap, double value)
            : base(value)
        {
            _lower = lower;
            _upper = upper;
            _snap = snap;
            Value = value;
        }
    }
}
