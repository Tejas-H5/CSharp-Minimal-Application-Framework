using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;
using System;
using System.Drawing;

namespace RenderingEngine.UI.Components
{
    public class UIText : UIComponent
    {
        public Color4 TextColor { get; set; }
        public string Text { get; internal set; }

        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }


        private PointF _caratPos = new PointF();

        public UIText(string text, Color4 textColor)
            : this(text, textColor, VerticalAlignment.Bottom, HorizontalAlignment.Left)
        {

        }

        public UIText(string text, Color4 textColor, VerticalAlignment vAlign, HorizontalAlignment hAlign)
        {
            TextColor = textColor;
            Text = text;
            VerticalAlignment = vAlign;
            HorizontalAlignment = hAlign;
        }

        public override void Draw(double deltaTime)
        {
            float scale = 1;
            float textHeight = scale * CTX.GetStringHeight(Text);
            float charHeight = scale * CTX.GetCharHeight('|');

            float startY = 0;
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    startY = _parent.Rect.Bottom + textHeight - charHeight;
                    break;
                case VerticalAlignment.Center:
                    startY = _parent.Rect.CenterY + textHeight / 2f - charHeight;
                    break;
                case VerticalAlignment.Top:
                    startY = _parent.Rect.Top - charHeight;
                    break;
            }


            int lineStart = 0;
            int lineEnd = 0;

            _caratPos = new PointF(0, startY);

            while (lineEnd < Text.Length)
            {
                lineEnd = Text.IndexOf('\n', lineStart);
                if (lineEnd == -1)
                    lineEnd = Text.Length;
                else
                    lineEnd++;

                float lineWidth = scale * CTX.GetStringWidth(Text, lineStart, lineEnd);
                
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        _caratPos.X = _parent.Rect.Left;
                        break;
                    case HorizontalAlignment.Center:
                        _caratPos.X = _parent.Rect.CenterX - lineWidth/2f;
                        break;
                    case HorizontalAlignment.Right:
                        _caratPos.X = _parent.Rect.Right - lineWidth;
                        break;
                }

                CTX.SetDrawColor(TextColor);
                _caratPos = CTX.DrawText(Text, lineStart, lineEnd, _caratPos.X, _caratPos.Y, scale);

                lineStart = lineEnd; 
            }
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
    }
}
