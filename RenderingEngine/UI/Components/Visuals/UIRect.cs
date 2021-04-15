using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.Visuals
{
    public class UIRect : UIComponent
    {
        private int _thickness;

        protected Color4 _initialColor;
        protected Color4 _color;
        protected Color4 _outlineColor;

        public UIRect(Color4 initColor)
            : this(initColor, new Color4(0, 0, 0, 1), 1)
        {
        }

        public UIRect(Color4 initColor, Color4 outlineColor, int thickness)
        {
            InitialColor = initColor;
            _outlineColor = outlineColor;
            _thickness = thickness;
        }

        public UIRect()
        {
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
            if (_color.A > 0.0001f)
            {
                CTX.SetDrawColor(_color);
                CTX.DrawRect(_parent.Rect);
            }

            if (_outlineColor.A > 0.0001f)
            {
                CTX.SetDrawColor(_outlineColor);
                CTX.DrawRectOutline(_thickness,
                    _parent.Rect.X0 + _thickness,
                    _parent.Rect.Y0 + _thickness,
                    _parent.Rect.X1 - _thickness,
                    _parent.Rect.Y1 - _thickness);
            }
        }
    }
}
