using MinimalAF.Rendering;
using MinimalAF;

namespace RenderingEngineVisualTests {
    class TextFontAtlasText : IRenderable {
        DrawableFont font;

        public TextFontAtlasText() {
            // font = new DrawableFont("Consolas", 16);
            font = new DrawableFont("Consolas", 44, "昨夜のコンサートは最高でした уилщертхуилоыхнлойк MR Worldwide 😎😎😎 💯 💯 💯 ");
        }

        float pos = 0;
        public void Render(FrameworkContext ctx) {
            pos += 50 * ctx.MouseWheelNotches;

            ctx.SetDrawColor(Color.Black);
            IM.DrawRect(ctx, 0, 0, ctx.VW, ctx.VH);

            var cX = ctx.VW * 0.5f;
            var cY = ctx.VH * 0.5f;
            var rect = new Rect(
                cX - font.Texture.Width / 2f, cY - font.Texture.Height / 2f,
                cX + font.Texture.Width / 2f, cY + font.Texture.Height / 2f
            );

            ctx.SetDrawColor(Color.Red);
            IM.DrawRectOutline(ctx, 2, rect);

            ctx.SetDrawColor(Color.White);
            ctx.SetTexture(font.Texture);
            IM.DrawRect(ctx, rect);
        }
    }
}
