using System;

namespace MinimalAF.Rendering {
    public class CircleDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        ImmediateMode2DDrawer<V> immediateModeDrawer;
        public CircleDrawer(ImmediateMode2DDrawer<V> immediateModeDrawer) {
            this.immediateModeDrawer = immediateModeDrawer;
        }


        public void Draw(float x0, float y0, float r, int edges) {
            immediateModeDrawer.Arc.Draw(x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void Draw(float x0, float y0, float r) {
            immediateModeDrawer.Arc.Draw(x0, y0, r, 0, MathF.PI * 2);
        }

        public void DrawOutline(float thickness, float x0, float y0, float r, int edges) {
            immediateModeDrawer.Arc.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void DrawOutline(float thickness, float x0, float y0, float r) {
            immediateModeDrawer.Arc.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2);
        }
    }
}
