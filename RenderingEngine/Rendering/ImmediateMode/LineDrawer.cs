using RenderingEngine.Rendering.ImmediateMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering.ImmediateMode
{
    public enum CapType
    {
        None,
        Circle
    }

    class LineDrawer
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
            _arcDrawer.DrawFilledArc(x0, y0, thickness, angle, angle + MathF.PI);
        }
    }
}
