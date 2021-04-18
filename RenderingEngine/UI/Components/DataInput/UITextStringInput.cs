using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextStringInput : UITextInput<string>
    {
        public UITextStringInput(StringProperty stringProperty, bool newLines, bool shouldClear)
            : base(stringProperty.Value, newLines, shouldClear)

        {
            _property = stringProperty;
        }

        protected override void OnPropertyChanged(string obj)
        {
            OnTextFinalizedSelf();
        }

        protected override bool TryParseText(string s, out string val)
        {
            val = s;
            return true;
        }
    }
}
