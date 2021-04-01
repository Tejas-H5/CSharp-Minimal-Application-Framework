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

            switch (cap)
            {
                case CapType.Circle:
                    {
                        DrawCircleCaps(x0, y0, x1, y1, thickness, dirX, dirY);

                        break;
                    }
                default:
                    break;
            }
        }


        private void DrawCircleCaps(float x0, float y0, float x1, float y1, float thickness, float dirX, float dirY)
        {
            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            _arcDrawer.DrawFilledArc(x0, y0, thickness, startAngle, startAngle + MathF.PI);

            startAngle += MathF.PI;
            _arcDrawer.DrawFilledArc(x1, y1, thickness, startAngle, startAngle + MathF.PI);
        }

    }
}
