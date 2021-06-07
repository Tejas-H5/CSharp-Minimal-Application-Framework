using System;

namespace MinimalAF.Rendering.ImmediateMode
{
    class LineDrawer : GeometryDrawer
    {
        QuadDrawer _quadDrawer;
        ArcDrawer _arcDrawer;

        public LineDrawer(QuadDrawer quadDrawer, ArcDrawer arcDrawer)
        {
            _quadDrawer = quadDrawer;
            _arcDrawer = arcDrawer;
        }

        public void DrawLine(float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            thickness /= 2;

            float dirX = x1 - x0;
            float dirY = y1 - y0;
            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            float perpX = -thickness * dirY / mag;
            float perpY = thickness * dirX / mag;


            _quadDrawer.DrawQuad(
                x0 + perpX, y0 + perpY,
                x0 - perpX, y0 - perpY,
                x1 - perpX, y1 - perpY,
                x1 + perpX, y1 + perpY
                );


            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            DrawCap(x0, y0, thickness, cap, startAngle);
            DrawCap(x1, y1, thickness, cap, startAngle + MathF.PI);
        }

        public void DrawLineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            thickness /= 2;

            float dirX = x1 - x0;
            float dirY = y1 - y0;
            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            float perpX = -(thickness + outlineThickness / 2f) * dirY / mag;
            float perpY = (thickness + outlineThickness / 2f) * dirX / mag;


            //Draw the outline using a NASCAR path
            _outlineDrawer.BeginPolyLine(x1 - perpX, y1 - perpY, outlineThickness, CapType.None);

            _outlineDrawer.DisableEnding();

            //_outlineDrawer.AppendToPolyLine(x0 - perpX, y0 - perpY);

            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            DrawCapOutline(outlineThickness, x0, y0, thickness, cap, startAngle);

            //_outlineDrawer.AppendToPolyLine(0, 0);

            //_outlineDrawer.AppendToPolyLine(x1 + perpX, y1 + perpY);

            _outlineDrawer.EnableEnding();
            DrawCapOutline(outlineThickness, x1, y1, thickness, cap, startAngle + MathF.PI);
        }


        public void DrawCap(float x0, float y0, float radius, CapType cap, float startAngle)
        {
            switch (cap)
            {
                case CapType.Circle:
                    {
                        DrawCircleCap(x0, y0, radius, startAngle);
                        break;
                    }
                default:
                    break;
            }
        }


        public void DrawCircleCap(float x0, float y0, float thickness, float angle)
        {
            _arcDrawer.DrawArc(x0, y0, thickness, angle, angle + MathF.PI);
        }


        public void DrawCapOutline(float outlineThickness, float x0, float y0, float radius, CapType cap, float startAngle)
        {
            switch (cap)
            {
                case CapType.Circle:
                    DrawCircleCapOutline(outlineThickness, x0, y0, radius, startAngle);
                    break;
                default:
                    DrawDefaultCapOutline(outlineThickness, x0, y0, radius, startAngle);
                    break;
            }
        }

        public void DrawDefaultCapOutline(float thickness, float x0, float y0, float radius, float angle)
        {
            _arcDrawer.DrawArcOutline(thickness, x0, y0, radius, angle, angle + MathF.PI, 1);
        }

        public void DrawCircleCapOutline(float thickness, float x0, float y0, float radius, float angle)
        {
            _arcDrawer.DrawArcOutline(thickness, x0, y0, radius, angle, angle + MathF.PI);
        }
    }
}
