using RenderingEngine.Datatypes;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.DataInput;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace UICodeGenerator.CustomEditors
{
    public class Color4Input : UIDataInput<Color4>
    {
        UIDataInput<float> _rInput;
        UIDataInput<float> _gInput;
        UIDataInput<float> _bInput;
        UIDataInput<float> _aInput;

        public Color4Input(UIDataInput<float> r, UIDataInput<float> g, UIDataInput<float> b, UIDataInput<float> a)
        {
            _property = new ColorProperty(new Color4());

            _rInput = r;
            _gInput = g;
            _bInput = b;
            _aInput = a;

            _rInput.Property.OnDataChanged += UpdateColorValue;
            _gInput.Property.OnDataChanged += UpdateColorValue;
            _bInput.Property.OnDataChanged += UpdateColorValue;
            _aInput.Property.OnDataChanged += UpdateColorValue;
        }

        void UpdateColorValue(float f)
        {
            _property.Value = new Color4(
                _rInput.Property.Value,
                _gInput.Property.Value,
                _bInput.Property.Value,
                _aInput.Property.Value
            );
        }

        public override UIComponent Copy()
        {
            return new Color4Input(
                (UIDataInput<float>)_rInput.Copy(),
                (UIDataInput<float>)_gInput.Copy(),
                (UIDataInput<float>)_bInput.Copy(),
                (UIDataInput<float>)_aInput.Copy()
            );
        }
    }
}
