using MinimalAF.Rendering;
using MinimalAF;

namespace RenderingEngineVisualTests {
    class TextFontAtlasText : IRenderable {
        DrawableFont font;

        public TextFontAtlasText() {
            // font = new DrawableFont("Consolas", 16);
            font = new DrawableFont("Consolas", 16, "昨夜のコンサートは最高でした");
        }

        float pos = 0;
        public void Render(FrameworkContext ctx) {
            pos += 50 * ctx.MouseWheelNotches;
            ctx.SetDrawColor(0, 0, 0, 1);
            ctx.SetTexture(font.Texture);

            var cX = ctx.VW * 0.5f;
            var cY = ctx.VH * 0.5f;
            var rect = new Rect(
                cX - font.Texture.Width / 2f, cY - font.Texture.Height / 2f,
                cX + font.Texture.Width / 2f, cY + font.Texture.Height / 2f
            );

            IM.DrawRect(ctx, rect);
        }
    }
}
