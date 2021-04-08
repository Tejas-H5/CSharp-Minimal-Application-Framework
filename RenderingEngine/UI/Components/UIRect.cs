using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UIRect : UIComponent
    {
        protected Color4 _initialColor;
        protected Color4 _color;

        public UIRect(Color4 initColor)
        {
            InitialColor = initColor;
        }

        public Color4 InitialColor {
            get { return _initialColor; }
            set {
                _initialColor = value;
                Color = value;
            }
        }

        public Color4 Color {
            get { return _color; }
            set { _color = value; }
        }

        public override void Draw(double deltaTime)
        {
            if (_color.A < 0.0001f)
                return;

            CTX.SetDrawColor(_color);
            CTX.DrawRect(_parent.Rect);
        }
    }
}
