using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public class UITextStringInput : UITextInput
    {
        public readonly StringProperty StringProperty = null;

        public UITextStringInput(StringProperty stringProperty, string placeholder, bool newLines, bool shouldClear)
            : base(placeholder, newLines, shouldClear)

        {
            StringProperty = stringProperty;
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            OnTextFinalized += OnTextFinalizedEvent;

            _textComponent.Text = StringProperty.Value.ToString();

            StringProperty.OnDataChanged += StringProperty_OnDataChanged;
        }

        private void StringProperty_OnDataChanged(string text)
        {
            if(text != null)
                _textComponent.Text = text;
            else
                _textComponent.Text = "";
        }

        private void OnTextFinalizedEvent()
        {
            StringProperty.Value = _textComponent.Text;
        }
    }
}
