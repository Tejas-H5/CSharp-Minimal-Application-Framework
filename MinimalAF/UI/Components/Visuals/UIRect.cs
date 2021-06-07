using MinimalAF.Datatypes;
using MinimalAF.Rendering;

namespace MinimalAF.UI
{
    public class UIRect : UIComponent
    {
        public int Thickness { get; set; }
        public Color4 OutlineColor { get; set; }

        public Color4 InitialColor {
            get { return _initialColor; }
            set {
                _initialColor = value;
                Color = value;
            }
        }

        protected Color4 _initialColor;
        protected Color4 _color;

        public Color4 Color {
            get { return _color; }
            set { _color = value; }
        }

        public UIRect(Color4 initColor)
            : this(initColor, new Color4(0, 0, 0, 1), 1)
        {
        }

        public UIRect(Color4 initColor, Color4 outlineColor, int thickness)
        {
            InitialColor = initColor;
            OutlineColor = outlineColor;
            Thickness = thickness;
        }

        public UIRect()
        {
        }

        public override void Draw(double deltaTime)
        {
            if (_color.A > 0.0001f)
            {
                CTX.SetDrawColor(_color);
                CTX.DrawRect(_parent.Rect);
            }

            if (OutlineColor.A > 0.0001f)
            {
                CTX.SetDrawColor(OutlineColor);
                CTX.DrawRectOutline(Thickness,
                    _parent.Rect.X0 + Thickness,
                    _parent.Rect.Y0 + Thickness,
                    _parent.Rect.X1 - Thickness,
                    _parent.Rect.Y1 - Thickness);
            }
        }

        public override UIComponent Copy()
        {
            return new UIRect(InitialColor, OutlineColor, Thickness);
        }
    }
}
