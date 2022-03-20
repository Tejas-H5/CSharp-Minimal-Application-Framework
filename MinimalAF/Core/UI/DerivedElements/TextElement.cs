using MinimalAF.Rendering;
using System.Drawing;

namespace MinimalAF {
    public class TextElement : Element {
        public Color4 TextColor {
            get; set;
        }
        public string String { get; set; } = "";

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
            String = text;
            VerticalAlignment = vAlign;
            HorizontalAlignment = hAlign;
            Font = fontName;
            FontSize = fontSize;
        }

        public override void OnRender() {
            if (String == null)
                return;

            SetFont(Font, FontSize);
            SetDrawColor(TextColor);

            float startX = 0, startY = 0;

            switch (VerticalAlignment) {
                case VerticalAlignment.Bottom:
                    startY = RelativeRect.Bottom;
                    break;
                case VerticalAlignment.Center:
                    startY = RelativeRect.CenterY;
                    break;
                case VerticalAlignment.Top:
                    startY = RelativeRect.Top;
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment) {
                case HorizontalAlignment.Left:
                    startX = RelativeRect.Left;
                    break;
                case HorizontalAlignment.Center:
                    startX = RelativeRect.CenterX;
                    break;
                case HorizontalAlignment.Right:
                    startX = RelativeRect.Right;
                    break;
                default:
                    break;
            }

            _caratPos = CTX.Text.Draw(String, startX, startY, HorizontalAlignment, VerticalAlignment, 1);
        }

        public override void OnLayout() {
			LayoutMargin(0);
        }

        public float TextWidth() {
            return GetStringWidth(String);
        }

        public PointF GetCaratPos() {
            return _caratPos;
        }

        public float GetCharacterHeight() {
            return CTX.Text.GetHeight('|');
        }
    }
}
