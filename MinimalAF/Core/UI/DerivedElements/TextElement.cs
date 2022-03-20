using MinimalAF.Rendering;
using System.Drawing;

namespace MinimalAF {
    public class TextElement : Element {
        public Color4 TextColor {
            get; set;
        }
        public string Text { get; set; } = "";

        public string Font { get; set; } = "";
        public int FontSize { get; set; } = -1;

        public VerticalAlignment VerticalAlignment {
            get; set;
        }
        public HorizontalAlignment HorizontalAlignment {
            get; set;
        }


        private PointF _caratPos = new PointF();

        public TextElement(string text, Color4 textColor)
            : this(text, textColor, "", -1, VerticalAlignment.Bottom, HorizontalAlignment.Left) {

        }

        public TextElement(string text, Color4 textColor, VerticalAlignment vAlign, HorizontalAlignment hAlign)
            : this(text, textColor, "", -1, vAlign, hAlign) {
        }

        public TextElement(string text, Color4 textColor, string fontName, int fontSize, VerticalAlignment vAlign, HorizontalAlignment hAlign) {
            TextColor = textColor;
            Text = text;
            VerticalAlignment = vAlign;
            HorizontalAlignment = hAlign;
            Font = fontName;
            FontSize = fontSize;
        }

        public override void OnRender() {
            if (Text == null)
                return;

            CTX.Text.SetFont(Font, FontSize);
            CTX.SetDrawColor(TextColor);

            float startX = 0, startY = 0;

            switch (VerticalAlignment) {
                case VerticalAlignment.Bottom:
                    startY = ScreenRect.Bottom;
                    break;
                case VerticalAlignment.Center:
                    startY = ScreenRect.CenterY;
                    break;
                case VerticalAlignment.Top:
                    startY = ScreenRect.Top;
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment) {
                case HorizontalAlignment.Left:
                    startX = ScreenRect.Left;
                    break;
                case HorizontalAlignment.Center:
                    startX = ScreenRect.CenterX;
                    break;
                case HorizontalAlignment.Right:
                    startX = ScreenRect.Right;
                    break;
                default:
                    break;
            }

            _caratPos = CTX.Text.Draw(Text, startX, startY, HorizontalAlignment, VerticalAlignment, 1);
        }

        internal float TextWidth() {
            //TODO: set the current font

            return GetStringWidth(Text);
        }

        public PointF GetCaratPos() {
            return _caratPos;
        }

        public float GetCharacterHeight() {
            //TODO: set the current font
            return CTX.Text.GetHeight('|');
        }
    }
}
