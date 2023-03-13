using MinimalAF.Rendering;
using MinimalAF;

namespace RenderingEngineVisualTests {
    class TextFontAtlasText : IRenderable {
        float pos = 0;
        public void Render(FrameworkContext ctx) {
            pos += 50 * ctx.MouseWheelNotches;
            ctx.SetDrawColor(0, 0, 0, 1);
            ctx.SetFont("Consolas", 16);

            var tex = CTX.InternalFontTexture;
            ctx.SetTexture(tex);

            var cX = ctx.VW * 0.5f;
            var cY = ctx.VH * 0.5f;
            var rect = new Rect(
                cX - tex.Width / 2f, cY - tex.Height / 2f,
                cX + tex.Width / 2f, cY + tex.Height / 2f
            );

            ctx.DrawRect(rect);
        }
    }
}
