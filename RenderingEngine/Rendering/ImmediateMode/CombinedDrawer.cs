using RenderingEngine.Datatypes.Geometric;

namespace RenderingEngine.Rendering.ImmediateMode
{
    class CombinedDrawer
    {
        TriangleDrawer _triangleDrawer;
        QuadDrawer _quadDrawer;
        NGonDrawer _ngonDrawer;
        PolyLineDrawer _polyLineDrawer;

        public QuadDrawer QuadDrawer { get { return _quadDrawer; } }

        ArcDrawer _arcDrawer;
        LineDrawer _lineDrawer;
        IGeometryOutput _outputStream;

        public CombinedDrawer(IGeometryOutput outputStream)
        {
            _outputStream = outputStream;

            _triangleDrawer = new TriangleDrawer(_outputStream);
            _quadDrawer = new QuadDrawer(_outputStream);
            _ngonDrawer = new NGonDrawer(_outputStream);

            _arcDrawer = new ArcDrawer(_ngonDrawer, circleEdgeLength: 5, maxCircleEdgeCount: 32);

            _lineDrawer = new LineDrawer(_quadDrawer, _arcDrawer);

            _polyLineDrawer = new PolyLineDrawer(_lineDrawer, _outputStream);

            _triangleDrawer.SetPolylineDrawer(_polyLineDrawer);
            _quadDrawer.SetPolylineDrawer(_polyLineDrawer);
            _ngonDrawer.SetPolylineDrawer(_polyLineDrawer);
            _arcDrawer.SetPolylineDrawer(_polyLineDrawer);
            _lineDrawer.SetPolylineDrawer(_polyLineDrawer);
        }

        public void AppendTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            _triangleDrawer.AppendTriangle(v1, v2, v3);
        }

        public void DrawTriangle(float x0, float y0, float x1, float y1, float x2, float y2,
                float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.5f, float v1 = 1f, float u2 = 1, float v2 = 0)
        {
            _triangleDrawer.DrawTriangle(x0, y0, x1, y1, x2, y2, u0, v0, u1, v1, u2, v2);
        }

        public void AppendQuad(Vertex v1, Vertex v2, Vertex v3, Vertex v4)
        {
            _quadDrawer.AppendQuad(v1, v2, v3, v4);
        }

        public void DrawRect(Rect2D rect)
        {
            _quadDrawer.DrawRect(rect);
        }

        public void DrawRect(Rect2D rect, Rect2D uvs)
        {
            _quadDrawer.DrawRect(rect, uvs);
        }

        public void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 1, float u1 = 1, float v1 = 0)
        {
            _quadDrawer.DrawRect(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        public void DrawRectOutline(float thickness, Rect2D rect)
        {
            _quadDrawer.DrawRectOutline(thickness, rect);
        }

        public void DrawRectOutline(float thickness, float x0, float y0, float x1, float y1)
        {
            _quadDrawer.DrawRectOutline(thickness, x0, y0, x1, y1);
        }

        public void DrawQuad(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3,
      float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.0f, float v1 = 1f, float u2 = 1, float v2 = 1, float u3 = 1, float v3 = 0)
        {
            _quadDrawer.DrawQuad(x0, y0, x1, y1, x2, y2, x3, y3, u0, v0, u1, v1, u2, v2, u3, v3);
        }

        public void DrawLine(float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            _lineDrawer.DrawLine(x0, y0, x1, y1, thickness, cap);
        }

        public void BeginPolyLine(float x0, float y0, float thickness, CapType cap)
        {
            _polyLineDrawer.BeginPolyLine(x0, y0, thickness, cap);
        }

        public void AppendToPolyLine(float x0, float y0)
        {
            _polyLineDrawer.AppendToPolyLine(x0, y0);
        }

        public void EndPolyLine(float x0, float y0)
        {
            _polyLineDrawer.EndPolyLine(x0, y0);
        }

        public void DrawCircle(float x0, float y0, float r, int edges)
        {
            _arcDrawer.DrawCircle(x0, y0, r, edges);
        }

        public void DrawCircle(float x0, float y0, float r)
        {
            _arcDrawer.DrawCircle(x0, y0, r);
        }

        public void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            _arcDrawer.DrawArc(xCenter, yCenter, radius, startAngle, endAngle);
        }

        public void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            _arcDrawer.DrawArc(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void DrawCircleOutline(float thickness, float x0, float y0, float r, int edges)
        {
            _arcDrawer.DrawCircleOutline(thickness, x0, y0, r, edges);
        }

        public void DrawCircleOutline(float thickness, float x0, float y0, float r)
        {
            _arcDrawer.DrawCircleOutline(thickness, x0, y0, r);
        }

        public void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            _arcDrawer.DrawArcOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            _arcDrawer.DrawArcOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle);
        }

        public void DrawLineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            _lineDrawer.DrawLineOutline(outlineThickness, x0, y0, x1, y1, thickness, cap);
        }
    }
}
