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

        private PointF _caratPos = new PointF();

        public UIText(string text, Color4 textColor)
        {
            TextColor = textColor;
            Text = text;
        }

        public override void Draw(double deltaTime)
        {
            float scale = 1;
            float textHeight = scale * CTX.GetStringHeight(Text);
            float charHeight = scale * CTX.GetCharHeight('|');
            float textWidth = scale * CTX.GetStringWidth(Text);

            CTX.SetDrawColor(TextColor);
            _caratPos = CTX.DrawText(Text, _parent.Rect.CenterX - textWidth / 2, _parent.Rect.CenterY + textHeight / 2 - charHeight, scale);
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
