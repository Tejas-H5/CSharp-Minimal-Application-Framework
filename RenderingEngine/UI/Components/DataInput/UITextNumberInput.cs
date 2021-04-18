using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextNumberInput : UITextInput<long>
    {
        public UITextNumberInput(IntegerProperty property, bool shouldClear)
            : base(property.Value, false, shouldClear)
        {
            _property = property;
        }

        protected override void OnPropertyChanged(long obj)
        {
        }

        protected override bool TryParseText(string s, out long val)
        {
            return long.TryParse(s, out val);
        }
    }
}
