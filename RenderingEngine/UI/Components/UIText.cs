using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components
{
    public class UIText : UIComponent
    {
        public Color4 TextColor { get; set; }
        public string Text { get; internal set; }

        public UIText(string text, Color4 textColor)
        {
            TextColor = textColor;
            Text = text;
        }

        public override void Draw(double deltaTime)
        {
            float scale = 1;
            float textHeight = scale * CTX.GetStringHeight(Text);
            float textWidth = scale * CTX.GetStringWidth(Text);

            CTX.SetDrawColor(TextColor);
            CTX.DrawText(Text, _parent.Rect.CenterX - textWidth / 2, _parent.Rect.CenterY - textHeight / 2, scale);
        }
    }
}
