using RenderingEngine.UI.Core;
using RenderingEngine.UI.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextNumberInput : UITextInput
    {
        public IntegerProperty IntProperty = null;

        readonly long _initValue;

        public UITextNumberInput(IntegerProperty property)
            : base("", false, true)
        {
            IntProperty = property;
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            OnTextFinalized += UITextNumberInput_OnTextFinalized;

            IntProperty.OnDataChanged += IntProperty_OnDataChanged;

            _textComponent.Text = IntProperty.Value.ToString();
        }

        private void IntProperty_OnDataChanged(long num)
        {
            _textComponent.Text = num.ToString();
        }

        private void UITextNumberInput_OnTextFinalized()
        {
            long num = _initValue;
            long.TryParse(_textComponent.Text, out num);

            IntProperty.Value = num;
        }
    }
}
