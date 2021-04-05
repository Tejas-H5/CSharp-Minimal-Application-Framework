using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering.ImmediateMode
{
    // It's like a stringbuilder, but for an OpenGL mesh
    // And with slightly different intentions.
    // When fillrate isn't a bottleneck, this is a great optimization
    class TriangleDrawer : GeometryDrawer
    {
        IGeometryOutput _outputStream;

        public TriangleDrawer(IGeometryOutput outputStream)
        {
            _outputStream = outputStream;
        }

        public void AppendTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            _outputStream.FlushIfRequired(3, 3);

            uint i1 = _outputStream.AddVertex(v1);
            uint i2 = _outputStream.AddVertex(v2);
            uint i3 = _outputStream.AddVertex(v3);

            _outputStream.MakeTriangle(i1, i2, i3);
        }


        public void DrawTriangle(
                float x0, float y0, float x1, float y1, float x2, float y2,
                float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.5f, float v1 = 1f, float u2 = 1, float v2 = 0)
        {
            AppendTriangle(
               new Vertex(new Vector3(x0, y0, 0), new Vector2(u0, v0)),
               new Vertex(new Vector3(x1, y1, 0), new Vector2(u1, v1)),
               new Vertex(new Vector3(x2, y2, 0), new Vector2(u2, v2))
               );
        }

        public void DrawTriangleOutline(float thickness, float x0, float y0, float x1, float y1, float x2, float y2)
        {
            _outlineDrawer.BeginPolyLine(x0, y0, thickness, CapType.None);
            _outlineDrawer.AppendToPolyLine(x1, y1);
            _outlineDrawer.AppendToPolyLine(x2, y2);
            _outlineDrawer.EndPolyLine(x0, y0);
        }
    }
}
