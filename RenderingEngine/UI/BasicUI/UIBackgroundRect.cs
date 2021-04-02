using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.BasicUI
{
    public class UIBackgroundRect
    {
        UIElement _parent;
        protected Color4 _color;
        public Color4 Color {
            get => _color;
            set {
                _color = value;
                HoverColor = value;
                ClickedColor = value;
            }
        }

        public Color4 HoverColor { get; set; }
        public Color4 ClickedColor { get; set; }

        public UIBackgroundRect(UIElement parent)
        {
            _parent = parent;
        }

        public void Draw(bool isMouseOver, bool isMouseDown)
        {
            Color4 color;
            if (isMouseOver)
            {
                if (isMouseDown)
                {
                    color = ClickedColor;
                }
                else
                {
                    color = HoverColor;
                }
            }
            else
            {
                color = Color;
            }

            if (color.A < 0.0001f)
                return;

            CTX.SetDrawColor(color);
            CTX.DrawRect(_parent.Rect);
        }
    }
}
