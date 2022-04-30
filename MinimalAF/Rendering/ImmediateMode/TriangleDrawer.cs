using OpenTK.Mathematics;

namespace MinimalAF.Rendering {
    // It's like a stringbuilder, but for an OpenGL mesh
    // And with slightly different intentions.
    // When fillrate isn't a bottleneck, this is a great optimization
    public class TriangleDrawer<V> where V : struct, IVertexPosition, IVertexUV {
        IGeometryOutput<V> outputStream;
        ImmediateMode2DDrawer<V> immediateModeDrawer;

        public TriangleDrawer(IGeometryOutput<V> outputStream, ImmediateMode2DDrawer<V> immediateModeDrawer) {
            this.outputStream = outputStream;
            this.immediateModeDrawer = immediateModeDrawer;
        }

        public void Draw(V v1, V v2, V v3) {
            outputStream.FlushIfRequired(3, 3);

            uint i1 = outputStream.AddVertex(v1);
            uint i2 = outputStream.AddVertex(v2);
            uint i3 = outputStream.AddVertex(v3);

            outputStream.MakeTriangle(i1, i2, i3);
        }


        public void Draw(
                float x0, float y0, float x1, float y1, float x2, float y2,
                float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.5f, float v1 = 1f, float u2 = 1, float v2 = 0) {
            V vertex1 = ImmediateMode2DDrawer<V>.CreateVertex(x0, y0, u0, v0);
            V vertex2 = ImmediateMode2DDrawer<V>.CreateVertex(x1, y1, u1, v1);
            V vertex3 = ImmediateMode2DDrawer<V>.CreateVertex(x2, y2, u2, v2);

            Draw(vertex1, vertex2, vertex3);
        }

        public void DrawOutline(float thickness, float x0, float y0, float x1, float y1, float x2, float y2) {
            immediateModeDrawer.NLine.Begin(x0, y0, thickness, CapType.None);
            immediateModeDrawer.NLine.Continue(x1, y1);
            immediateModeDrawer.NLine.Continue(x2, y2);
            immediateModeDrawer.NLine.End(x0, y0);
        }
    }
}
