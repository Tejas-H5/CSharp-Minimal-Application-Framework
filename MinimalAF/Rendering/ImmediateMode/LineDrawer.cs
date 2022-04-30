using System;

namespace MinimalAF.Rendering {
    public class LineDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        ImmediateMode2DDrawer<V> immediateModeDrawer;
        public LineDrawer(ImmediateMode2DDrawer<V> immediateModeDrawer) {
            this.immediateModeDrawer = immediateModeDrawer;
        }

        public void Draw(float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            thickness /= 2;

            float dirX = x1 - x0;
            float dirY = y1 - y0;
            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            float perpX = -thickness * dirY / mag;
            float perpY = thickness * dirX / mag;


            immediateModeDrawer.Quad.Draw2D(
                x0 + perpX, y0 + perpY,
                x0 - perpX, y0 - perpY,
                x1 - perpX, y1 - perpY,
                x1 + perpX, y1 + perpY
                );


            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            DrawCap(x0, y0, thickness, cap, startAngle);
            DrawCap(x1, y1, thickness, cap, startAngle + MathF.PI);
        }

        public void DrawOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            thickness /= 2;

            float dirX = x1 - x0;
            float dirY = y1 - y0;
            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            float perpX = -(thickness + outlineThickness / 2f) * dirY / mag;
            float perpY = (thickness + outlineThickness / 2f) * dirX / mag;


            //Draw the outline using a NASCAR path
            immediateModeDrawer.NLine.Begin(x1 - perpX, y1 - perpY, outlineThickness, CapType.None);

            immediateModeDrawer.NLine.DisableEnding();

            //immediateModeDrawer.NLine.AppendToPolyLine(x0 - perpX, y0 - perpY);

            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            DrawCapOutline(outlineThickness, x0, y0, thickness, cap, startAngle);

            //immediateModeDrawer.NLine.AppendToPolyLine(0, 0);

            //immediateModeDrawer.NLine.AppendToPolyLine(x1 + perpX, y1 + perpY);

            immediateModeDrawer.NLine.EnableEnding();
            DrawCapOutline(outlineThickness, x1, y1, thickness, cap, startAngle + MathF.PI);
        }


        public void DrawCap(float x0, float y0, float radius, CapType cap, float startAngle) {
            switch (cap) {
                case CapType.Circle: {
                        DrawCircleCap(x0, y0, radius, startAngle);
                        break;
                    }
                default:
                    break;
            }
        }


        public void DrawCircleCap(float x0, float y0, float thickness, float angle) {
            immediateModeDrawer.Arc.Draw(x0, y0, thickness, angle, angle + MathF.PI);
        }


        public void DrawCapOutline(float outlineThickness, float x0, float y0, float radius, CapType cap, float startAngle) {
            switch (cap) {
                case CapType.Circle:
                    DrawCircleCapOutline(outlineThickness, x0, y0, radius, startAngle);
                    break;
                default:
                    DrawDefaultCapOutline(outlineThickness, x0, y0, radius, startAngle);
                    break;
            }
        }

        public void DrawDefaultCapOutline(float thickness, float x0, float y0, float radius, float angle) {
            immediateModeDrawer.Arc.DrawOutline(thickness, x0, y0, radius, angle, angle + MathF.PI, 1);
        }

        public void DrawCircleCapOutline(float thickness, float x0, float y0, float radius, float angle) {
            immediateModeDrawer.Arc.DrawOutline(thickness, x0, y0, radius, angle, angle + MathF.PI);
        }
    }
}
