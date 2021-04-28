using RenderingEngine.Datatypes.ObserverPattern;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    //TODO: Esc to cancel out of an input
    //might have to use another string rather than the string inside the TextComponent
    [RequiredComponents(typeof(UIRect), typeof(UIMouseListener), typeof(UIText))]
    public class UICheckbox : UIDataInput<bool>
    {
        public UICheckbox(Property<bool> prop)
        {
            _property = prop;
        }

        UIMouseListener _mouseListner;
        UIText _textComponent;
        UIRect _rect;

        public override void SetParent(UIElement parent)
        {
            if (_mouseListner != null)
            {
                _mouseListner.OnMouseOver -= OnMouseOver;
            }

            base.SetParent(parent);

            _mouseListner = _parent.GetComponentOfType<UIMouseListener>();
            _textComponent = _parent.GetComponentInChildrenOfType<UIText>();
            _rect = _parent.GetComponentInChildrenOfType<UIRect>();

            _mouseListner.OnMouseOver += OnMouseOver;
        }

        private void OnMouseOver()
        {
            if (Input.IsMouseClicked(MouseButton.Left))
            {
                bool wantedValue = !_property.Value;

                if (wantedValue)
                {
                    _textComponent.Text = "True";
                    _textComponent.TextColor = new Datatypes.Color4(1, 1);
                    _rect.Color = new Datatypes.Color4(0, 1);
                }
                else
                {
                    _textComponent.Text = "False";
                    _textComponent.TextColor = new Datatypes.Color4(0, 1);
                    _rect.Color = new Datatypes.Color4(0, 0);
                }

                _property.Value = wantedValue;
            }
        }


        public override UIComponent Copy()
        {
            return new UICheckbox(_property.Copy());
        }
    }
}
