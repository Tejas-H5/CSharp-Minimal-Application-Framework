using OpenTK.Mathematics;

namespace MinimalAF.Rendering {
    public class QuadDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        IGeometryOutput<V> outputStream;

        public QuadDrawer(IGeometryOutput<V> outputStream) {
            this.outputStream = outputStream;
        }

        /// <summary>
        /// Assumes the verts are defined clockwise
        /// </summary>
        public void Draw(V v1, V v2, V v3, V v4) {
            outputStream.FlushIfRequired(4, 6);

            uint i1 = outputStream.AddVertex(v1);
            uint i2 = outputStream.AddVertex(v2);
            uint i3 = outputStream.AddVertex(v3);
            uint i4 = outputStream.AddVertex(v4);

            outputStream.MakeTriangle(i1, i2, i3);
            outputStream.MakeTriangle(i3, i4, i1);
        }



        public void Draw2D(
            float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3,
      float u0 = 0f, float v0 = 0f, float u1 = 1f, float v1 = 0f, float u2 = 1f, float v2 = 1f, float u3 = 0f, float v3 = 1f) {
            Draw(
               ImmediateMode2DDrawer<V>.CreateVertex(x0, y0, u0, v0),
               ImmediateMode2DDrawer<V>.CreateVertex(x1, y1, u1, v1),
               ImmediateMode2DDrawer<V>.CreateVertex(x2, y2, u2, v2),
               ImmediateMode2DDrawer<V>.CreateVertex(x3, y3, u3, v3)
            );
        }
    }
}
