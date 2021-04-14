using RenderingEngine.UI.Core;
using RenderingEngine.UI.Properties;
using RenderingEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UITextFloatInput : UITextInput
    {
        public readonly FloatProperty FloatProperty = new FloatProperty();

        readonly double _initValue;


        public UITextFloatInput(double value) 
        {
            FloatProperty.Value = _initValue = value;
        }

        public UITextFloatInput(double value, double lower, double upper, double snap)
        {
            FloatProperty.Lock();
            FloatProperty.Lower = lower;
            FloatProperty.Upper = upper;
            FloatProperty.Snap = snap;
            FloatProperty.Value = _initValue = value;
            FloatProperty.Unlock();
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            base.OnTextFinalized += UITextNumberInput_OnTextChanged;
            base.OnTextChanged += UITextFloatInput_OnTextChanged;
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

            _textComponent.Text = num.ToString();
            FloatProperty.Value = num;
        }

    }
}
