using MinimalAF.Rendering;
using MinimalAF;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Tests that font loading is working. It is failing at the moment.",
        tags: "2D, text"
    )]
    class TextFontAtlasText : IRenderable {
        public TextFontAtlasText(FrameworkContext ctx) {
            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "text font atlas test";
            w.RenderFrequency = 120;//  60;
            w.UpdateFrequency = 120;//  120;

            ctx.SetClearColor(Color.White);
        }

        float pos = 0;
        public void Render(FrameworkContext ctx) {
            pos += 50 * ctx.Window.MouseWheelNotches;
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
