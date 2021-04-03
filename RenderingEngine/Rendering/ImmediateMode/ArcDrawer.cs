using RenderingEngine.Rendering;
using RenderingEngine.Rendering.ImmediateMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering.ImmediateMode
{
    class ArcDrawer
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

        public void DrawFilledCircle(float x0, float y0, float r, int edges)
        {
            DrawFilledArc(x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public void DrawFilledCircle(float x0, float y0, float r)
        {
            DrawFilledArc(x0, y0, r, 0, MathF.PI * 2);
        }

        public void DrawFilledArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            float deltaAngle = _circleEdgeLength / radius;
            int edgeCount = (int)((endAngle - startAngle) / deltaAngle) + 1;

            if (edgeCount > _maxCircleEdgeCount)
            {
                edgeCount = _maxCircleEdgeCount;
            }

            DrawFilledArc(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }


        public void DrawFilledArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
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
    }
}
