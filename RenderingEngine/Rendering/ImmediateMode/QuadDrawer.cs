using OpenTK.Mathematics;
using RenderingEngine.Datatypes.Geometric;

namespace RenderingEngine.Rendering.ImmediateMode
{
    class QuadDrawer : GeometryDrawer
    {
        IGeometryOutput _outputStream;

        public QuadDrawer(IGeometryOutput outputStream)
        {
            _outputStream = outputStream;
        }

        public void AppendQuad(Vertex v1, Vertex v2, Vertex v3, Vertex v4)
        {
            _outputStream.FlushIfRequired(4, 6);

            uint i1 = _outputStream.AddVertex(v1);
            uint i2 = _outputStream.AddVertex(v2);
            uint i3 = _outputStream.AddVertex(v3);
            uint i4 = _outputStream.AddVertex(v4);

            _outputStream.MakeTriangle(i1, i2, i3);
            _outputStream.MakeTriangle(i3, i4, i1);
        }

        public void DrawQuad(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3,
      float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.0f, float v1 = 1f, float u2 = 1, float v2 = 1, float u3 = 1, float v3 = 0)
        {
            AppendQuad(
               new Vertex(new Vector3(x0, y0, 0), new Vector2(u0, v0)),
               new Vertex(new Vector3(x1, y1, 0), new Vector2(u1, v1)),
               new Vertex(new Vector3(x2, y2, 0), new Vector2(u2, v2)),
               new Vertex(new Vector3(x3, y3, 0), new Vector2(u3, v3))
            );
        }

        public void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 1, float u1 = 1, float v1 = 0)
        {
            DrawQuad(
                x0, y0,
                x0, y1,
                x1, y1,
                x1, y0,
                u0, v0,
                u0, v1,
                u1, v1,
                u1, v0
                );
        }

        public void DrawRect(Rect2D rect, Rect2D uvs)
        {
            DrawRect(rect.X0, rect.Y0, rect.X1, rect.Y1, uvs.X0, uvs.Y0, uvs.X1, uvs.Y1);
        }

        public void DrawRect(Rect2D rect)
        {
            DrawRect(rect.X0, rect.Y0, rect.X1, rect.Y1, 0, 1, 1, 0);
        }

        public void DrawRectOutline(float thickness, Rect2D rect)
        {
            DrawRectOutline(thickness, rect.X0, rect.Y0, rect.X1, rect.Y1);
        }

        public void DrawRectOutline(float thickness, float x0, float y0, float x1, float y1)
        {
            DrawRect(x0 - thickness, y0 - thickness, x1, y0);
            DrawRect(x0, y1, x1 + thickness, y1 + thickness);

            DrawRect(x0 - thickness, y0, x0, y1 + thickness);
            DrawRect(x1, y0 - thickness, x1 + thickness, y1);
        }
    }
}
