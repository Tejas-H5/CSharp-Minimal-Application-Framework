using RenderingEngine.Datatypes;
using RenderingEngine.Util;
using System;

namespace RenderingEngine.UI.Properties
{
    public class FloatProperty : ObservableData
    {
        double _lower, _upper, _snap;
        private double _value;
        public double Value { get => _value; set => SetValue(value); }

        public double Lower {
            get { return _lower; }
            set {
                _lower = value;

                SetValue(_value);
            }
        }

        public double Upper {
            get { return _upper; }
            set {
                _upper = value;

                SetValue(_value);
            }
        }

        public double Snap {
            get { return _snap; }
            set {
                _snap = value;
                SetValue(_value);
            }
        }

        private void SetValue(double num)
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
            DataChanged();
        }


        public FloatProperty()
            : this(double.MinValue, double.MaxValue, -1, 0)
        {
        }

        public FloatProperty(double lower, double upper, double snap, double value)
        {
            _lower = lower;
            _upper = upper;
            _snap = snap;
            Value = value;
        }
    }
}
