//using OpenTK.Mathematics;

//namespace MinimalAF {
//    public class TextElement : Element {
//        public Color TextColor {
//            get; set;
//        }
//        public string String { get; set; } = "";

//        public string Font { get; set; } = "";
//        public int FontSize { get; set; } = -1;

//        float _charHeight;

//        public VAlign VerticalAlignment {
//            get; set;
//        }
//        public HAlign HorizontalAlignment {
//            get; set;
//        }


//        private Vector2 caratPos = new Vector2();

//        public TextElement(string text, Color textColor)
//            : this(text, textColor, "", -1, VAlign.Bottom, HAlign.Left) {

//        }

//        public TextElement(string text, Color textColor, VAlign vAlign = VAlign.Bottom, HAlign hAlign = HAlign.Left)
//            : this(text, textColor, "", -1, vAlign, hAlign) {
//        }

//        public TextElement(string text, Color textColor, string fontName, int fontSize, VAlign vAlign = VAlign.Bottom, HAlign hAlign = HAlign.Left) {
//            TextColor = textColor;
//            String = text;
//            VerticalAlignment = vAlign;
//            HorizontalAlignment = hAlign;
//            Font = fontName;
//            FontSize = fontSize;
//        }

//        public override void OnRender() {
//            if (String == null)
//                return;

//            SetFont(Font, FontSize);
//            ctx.SetDrawColor(TextColor);

//            float startX = 0, startY = 0;

//            switch (VerticalAlignment) {
//                case VAlign.Bottom:
//                    startY = 0;
//                    break;
//                case VAlign.Center:
//                    startY = ctx.Height * 0.5f;
//                    break;
//                case VAlign.Top:
//                    startY = ctx.Height * 1.0f;
//                    break;
//                default:
//                    break;
//            }

//            switch (HorizontalAlignment) {
//                case HAlign.Left:
//                    startX = 0;
//                    break;
//                case HAlign.Center:
//                    startX = ctx.Width * 0.5f;
//                    break;
//                case HAlign.Right:
//                    startX = ctx.Width * 1f;
//                    break;
//                default:
//                    break;
//            }

//            caratPos = DrawText(String, startX, startY, HorizontalAlignment, VerticalAlignment, 1);
//        }

//        public override Rect DefaultRect(float parentWidth, float parentHeight) {
//            SetFont(Font, FontSize);
//            _charHeight = GetCharHeight();

//            return new Rect(0, 0, parentWidth, _charHeight + 5);
//        }

//        public float TextWidth() {
//            return GetStringWidth(String);
//        }

//        public Vector2 GetCaratPos() {
//            return caratPos;
//        }

//        public float GetCharacterHeight() {
//            SetFont(Font, FontSize);
//            return GetCharHeight();
//        }
//    }
//}
