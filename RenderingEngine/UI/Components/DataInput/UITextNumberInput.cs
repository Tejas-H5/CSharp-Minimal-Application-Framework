using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextNumberInput : UITextInput<int>
    {
        public UITextNumberInput(Property<int> property, bool shouldClear)
            : base(property.Value, false, shouldClear)
        {
            _property = property;
        }

        public override UIComponent Copy()
        {
            return new UITextNumberInput(_property.Copy(), _shouldClear);
        }

        protected override void OnPropertyChanged(int obj)
        {
        }

        protected override bool TryParseText(string s, out int val)
        {
            return int.TryParse(s, out val);
        }
    }
}
