using RenderingEngine.UI.Core;
using RenderingEngine.UI.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UITextNumberInput : UITextInput
    {
        public readonly IntegerProperty Integer = new IntegerProperty();

        public UITextNumberInput(long value)
        {
            Integer.Value = _initValue = value;
        }

        readonly long _initValue;

        public UITextNumberInput(long value, long lower, long upper, long snap)
        {
            Integer.Lock();
            Integer.Lower = lower;
            Integer.Upper = upper;
            Integer.Snap = snap;
            Integer.Value = _initValue = value;
            Integer.Unlock();
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            OnTextFinalized += UITextNumberInput_OnTextFinalized;
            OnTextChanged += UITextFloatInput_OnTextChanged;
        }

        private void UITextFloatInput_OnTextChanged()
        {
            string s = _textComponent.Text;
            if (s.Length > 0 && s[s.Length - 1] == '\n')
            {
                _textComponent.Text = s.Substring(0, s.Length - 1);
                EndTyping();
            }
        }

        private void UITextNumberInput_OnTextFinalized()
        {
            long num = _initValue;
            long.TryParse(_textComponent.Text, out num);

            _textComponent.Text = num.ToString();
            Integer.SetValue(num);
        }
    }
}
