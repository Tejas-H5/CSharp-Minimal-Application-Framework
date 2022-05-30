using System;

namespace MinimalAF.Rendering {
    public class ArcDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        int circleEdgeLength;
        int maxCircleEdgeCount;
        ImmediateMode2DDrawer<V> immediateModeDrawer;

        public ArcDrawer(int circleEdgeLength, int maxCircleEdgeCount, ImmediateMode2DDrawer<V> immediateModeDrawer) {
            this.circleEdgeLength = circleEdgeLength;
            this.maxCircleEdgeCount = maxCircleEdgeCount;
            this.immediateModeDrawer = immediateModeDrawer;
        }


        public void Draw(float xCenter, float yCenter, float radius, float startAngle, float endAngle) {
            int edgeCount = GetEdgeCount(radius, startAngle, endAngle);

            Draw(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        private int GetEdgeCount(float radius, float startAngle, float endAngle) {
            float deltaAngle = circleEdgeLength / radius;
            int edgeCount = (int)((endAngle - startAngle) / deltaAngle) + 1;

            if (edgeCount > maxCircleEdgeCount) {
                edgeCount = maxCircleEdgeCount;
            }

            return edgeCount;
        }

        public void Draw(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            if (edgeCount < 0)
                return;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            immediateModeDrawer.NGon.Begin(xCenter, yCenter, edgeCount + 2);

            for (float angle = endAngle; angle > startAngle - deltaAngle + 0.001f; angle -= deltaAngle) {
                float X = xCenter + radius * MathF.Sin(angle);
                float Y = yCenter + radius * MathF.Cos(angle);

                immediateModeDrawer.NGon.Continue(X, Y);
            }

            immediateModeDrawer.NGon.End();
        }


        public void DrawOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle) {
            int edges = GetEdgeCount(r, startAngle, endAngle);
            DrawOutline(thickness, x0, y0, r, startAngle, endAngle, edges);
        }

        public void DrawOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            if (edgeCount < 0)
                return;

            thickness /= 2.0f;
            radius += thickness / 2f;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            bool first = true;
            for (float angle = startAngle; angle < endAngle + deltaAngle - 0.001f; angle += deltaAngle) {
                float X = xCenter + radius * MathF.Sin(angle);
                float Y = yCenter + radius * MathF.Cos(angle);

                if (first) {
                    immediateModeDrawer.NLine.Begin(X, Y, thickness, CapType.None);
                    first = false;
                } else if (angle + deltaAngle < endAngle + 0.00001f) {
                    immediateModeDrawer.NLine.Continue(X, Y);
                } else {
                    immediateModeDrawer.NLine.End(X, Y);
                }
            }
        }
    }
}
