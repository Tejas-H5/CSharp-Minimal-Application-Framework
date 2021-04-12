using System;

namespace RenderingEngine.Rendering.ImmediateMode
{
    class ArcDrawer : GeometryDrawer
    {
        NGonDrawer _ngonDrawer;
        int _circleEdgeLength;
        int _maxCircleEdgeCount;

        public ArcDrawer(NGonDrawer ngonDrawer, int circleEdgeLength, int maxCircleEdgeCount)
        {
            _ngonDrawer = ngonDrawer;
            _circleEdgeLength = circleEdgeLength;
            _maxCircleEdgeCount = maxCircleEdgeCount;
        }

        public void DrawCircle(float x0, float y0, float r, int edges)
        {
            DrawArc(x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void DrawCircle(float x0, float y0, float r)
        {
            DrawArc(x0, y0, r, 0, MathF.PI * 2);
        }

        public void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            int edgeCount = GetEdgeCount(radius, startAngle, endAngle);

            DrawArc(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        private int GetEdgeCount(float radius, float startAngle, float endAngle)
        {
            float deltaAngle = _circleEdgeLength / radius;
            int edgeCount = (int)((endAngle - startAngle) / deltaAngle) + 1;

            if (edgeCount > _maxCircleEdgeCount)
            {
                edgeCount = _maxCircleEdgeCount;
            }

            return edgeCount;
        }

        public void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            if (edgeCount < 0)
                return;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            _ngonDrawer.BeginNGon(new Vertex(xCenter, yCenter, 0), edgeCount + 2);

            for (float angle = startAngle; angle < endAngle + deltaAngle - 0.001f; angle += deltaAngle)
            {
                float X = xCenter + radius * MathF.Sin(angle);
                float Y = yCenter + radius * MathF.Cos(angle);

                _ngonDrawer.AppendToNGon(new Vertex(X, Y, 0));
            }

            _ngonDrawer.EndNGon();
        }

        public void DrawCircleOutline(float thickness, float x0, float y0, float r, int edges)
        {
            DrawArcOutline(thickness, x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void DrawCircleOutline(float thickness, float x0, float y0, float r)
        {
            DrawArcOutline(thickness, x0, y0, r, 0, MathF.PI * 2);
        }
        public void DrawArcOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle)
        {
            int edges = GetEdgeCount(r, startAngle, endAngle);
            DrawArcOutline(thickness, x0, y0, r, startAngle, endAngle, edges);
        }

        public void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            if (edgeCount < 0)
                return;

            radius += thickness / 2 - 0.1f;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            bool first = true;
            for (float angle = startAngle; angle < endAngle + deltaAngle - 0.001f; angle += deltaAngle)
            {
                float X = xCenter + radius * MathF.Sin(angle);
                float Y = yCenter + radius * MathF.Cos(angle);

                if (first)
                {
                    _outlineDrawer.BeginPolyLine(X, Y, thickness, CapType.None);
                    first = false;
                }
                else if (angle + deltaAngle < endAngle + 0.00001f)
                {
                    _outlineDrawer.AppendToPolyLine(X, Y);
                }
                else
                {
                    _outlineDrawer.EndPolyLine(X, Y);
                }
            }
        }
    }
}
