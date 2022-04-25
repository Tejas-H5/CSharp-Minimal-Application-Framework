﻿using OpenTK.Mathematics;

namespace MinimalAF.Rendering.ImmediateMode {
    public class QuadDrawer {
        IGeometryOutput outputStream;

        public QuadDrawer(IGeometryOutput outputStream) {
            this.outputStream = outputStream;
        }

        /// <summary>
        /// Assumes the verts are defined clockwise
        /// </summary>
        public void Draw(Vertex v1, Vertex v2, Vertex v3, Vertex v4) {
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
               new Vertex(new Vector3(x0, y0, CTX.Current2DDepth), new Vector2(u0, v0)),
               new Vertex(new Vector3(x1, y1, CTX.Current2DDepth), new Vector2(u1, v1)),
               new Vertex(new Vector3(x2, y2, CTX.Current2DDepth), new Vector2(u2, v2)),
               new Vertex(new Vector3(x3, y3, CTX.Current2DDepth), new Vector2(u3, v3))
            );
        }
    }
}
