using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using RenderingEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextFloatInput : UITextInput<double>
    {
        public UITextFloatInput(FloatProperty property, bool shouldClear)
            : base(property.Value, false, shouldClear)
        {
            _property = property;
        }

        protected override void OnPropertyChanged(double obj)
        {
        }

        protected override bool TryParseText(string s, out double val)
        {
            return double.TryParse(s, out val);
        }
    }
}
