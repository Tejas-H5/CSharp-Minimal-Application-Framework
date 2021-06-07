using MinimalAF.Datatypes;
using MinimalAF.Rendering;
using MinimalAF.UI.Core;
using System;
using System.Drawing;

namespace MinimalAF.UI.Components.Visuals
{
    public class UIText : UIComponent
    {
        public Color4 TextColor { get; set; }
        public string Text { get; set; } = "";

        public string Font { get; set; } = "";
        public int FontSize { get; set; } = -1;

        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }


        private PointF _caratPos = new PointF();

        public UIText(string text, Color4 textColor)
            : this(text, textColor, "", -1, VerticalAlignment.Bottom, HorizontalAlignment.Left)
        {

        }

        public UIText(string text, Color4 textColor, VerticalAlignment vAlign, HorizontalAlignment hAlign)
            : this(text, textColor, "", -1, vAlign, hAlign)
        {
        }

        public UIText(string text, Color4 textColor, string fontName, int fontSize, VerticalAlignment vAlign, HorizontalAlignment hAlign)
        {
            TextColor = textColor;
            Text = text;
            VerticalAlignment = vAlign;
            HorizontalAlignment = hAlign;
            Font = fontName;
            FontSize = fontSize;
        }

        public override void Draw(double deltaTime)
        {
            if (Text == null)
                return;

            CTX.SetCurrentFont(Font, FontSize);
            CTX.SetDrawColor(TextColor);

            float startX = 0, startY = 0;

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    startY = _parent.Rect.Bottom;
                    break;
                case VerticalAlignment.Center:
                    startY = _parent.Rect.CenterY;
                    break;
                case VerticalAlignment.Top:
                    startY = _parent.Rect.Top;
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    startX = _parent.Rect.Left;
                    break;
                case HorizontalAlignment.Center:
                    startX = _parent.Rect.CenterX;
                    break;
                case HorizontalAlignment.Right:
                    startX = _parent.Rect.Right;
                    break;
                default:
                    break;
            }

            _caratPos = CTX.DrawTextAligned(Text, startX, startY, HorizontalAlignment, VerticalAlignment, 1);
        }

        internal float TextWidth()
        {
            //TODO: set the current font

            return CTX.GetStringWidth(Text);
        }

        public PointF GetCaratPos()
        {
            return _caratPos;
        }

        public float GetCharacterHeight()
        {
            //TODO: set the current font
            return CTX.GetCharHeight('|');
        }

        public override UIComponent Copy()
        {
            return new UIText(Text, TextColor, Font, FontSize, VerticalAlignment, HorizontalAlignment);
        }
    }
}
