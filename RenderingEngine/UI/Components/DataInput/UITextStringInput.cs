using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextStringInput : UITextInput<string>
    {
        public UITextStringInput(Property<string> stringProperty, bool newLines, bool shouldClear)
            : base(stringProperty.Value, newLines, shouldClear)

        {
            _property = stringProperty;
        }

        public override UIComponent Copy()
        {
            return new UITextStringInput(_property.Copy(), _acceptsNewLine, _shouldClear);
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
