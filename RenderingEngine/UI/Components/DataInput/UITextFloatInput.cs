using RenderingEngine.UI.Core;
using RenderingEngine.UI.Properties;
using RenderingEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextFloatInput : UITextInput
    {
        public FloatProperty FloatProperty = null;

        readonly double _initValue;

        public UITextFloatInput(FloatProperty property, bool shouldClear)
            : base("", false, shouldClear)
        {
            FloatProperty = property;
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            OnTextFinalized += UITextNumberInput_OnTextChanged;
            OnTextChanged += UITextFloatInput_OnTextChanged;

            _textComponent.Text = FloatProperty.Value.ToString();
            FloatProperty.OnDataChanged += FloatProperty_OnDataChanged;
        }

        private void FloatProperty_OnDataChanged(double val)
        {
            _textComponent.Text = val.ToString();
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

        private void UITextNumberInput_OnTextChanged()
        {
            double num = _initValue;
            double.TryParse(_textComponent.Text, out num);

            FloatProperty.Value = num;
        }
    }
}
