using System;

namespace MinimalAF.Rendering.ImmediateMode
{
	public class ArcDrawer : GeometryDrawer
    {
        int _circleEdgeLength;
        int _maxCircleEdgeCount;

        public ArcDrawer(int circleEdgeLength, int maxCircleEdgeCount)
        {
			_circleEdgeLength = circleEdgeLength;
            _maxCircleEdgeCount = maxCircleEdgeCount;
        }



        public void Draw(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            int edgeCount = GetEdgeCount(radius, startAngle, endAngle);

            Draw(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
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

        public void Draw(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            if (edgeCount < 0)
                return;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            CTX.NGon.Begin(new Vertex(xCenter, yCenter, 0), edgeCount + 2);

            for (float angle = startAngle; angle < endAngle + deltaAngle - 0.001f; angle += deltaAngle)
            {
                float X = xCenter + radius * MathF.Sin(angle);
                float Y = yCenter + radius * MathF.Cos(angle);

                CTX.NGon.Continue(new Vertex(X, Y, 0));
            }

            CTX.NGon.End();
        }


        public void DrawOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle)
        {
            int edges = GetEdgeCount(r, startAngle, endAngle);
            DrawOutline(thickness, x0, y0, r, startAngle, endAngle, edges);
        }

        public void DrawOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
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
                    CTX.NLine.Begin(X, Y, thickness, CapType.None);
                    first = false;
                }
                else if (angle + deltaAngle < endAngle + 0.00001f)
                {
                    CTX.NLine.Continue(X, Y);
                }
                else
                {
                    CTX.NLine.End(X, Y);
                }
            }
        }
    }
}
