using MinimalAF.UI;
using MinimalAF.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI
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

        protected override bool TryParseText(string s, out int val)
        {
            return int.TryParse(s, out val);
        }
    }
}
