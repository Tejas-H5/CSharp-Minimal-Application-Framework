using System;

namespace MinimalAF.Rendering.ImmediateMode {
    public class CircleDrawer {
        RenderContext ctx;

        public CircleDrawer(RenderContext context) {
            ctx = context;
        }


        public void Draw(float x0, float y0, float r, int edges) {
            ctx.Arc.Draw(x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void Draw(float x0, float y0, float r) {
            ctx.Arc.Draw(x0, y0, r, 0, MathF.PI * 2);
        }

        public void DrawOutline(float thickness, float x0, float y0, float r, int edges) {
            ctx.Arc.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void DrawOutline(float thickness, float x0, float y0, float r) {
            ctx.Arc.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2);
        }
    }
}
