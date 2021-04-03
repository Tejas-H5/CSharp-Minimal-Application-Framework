using RenderingEngine.Datatypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.UI
{
    public static class Intersections
    {
        public static bool IsInside(PointF p, Rect2D a)
        {
            return IsInside(p.X, p.Y, a.X0, a.Y0, a.X1, a.Y1);
        }

        public static bool IsInside(float x, float y, Rect2D a)
        {
            return IsInside(x, y, a.X0, a.Y0, a.X1, a.Y1);
        }

        public static bool IsInside(float x, float y, float left, float bottom, float right, float top)
        {
            if (x > left && x < right)
            {
                if (y < top && y > bottom)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInsideCircle(float x, float y, float circleX, float circleY, float radius)
        {
            return ((x - circleX) * (x - circleX) + (y - circleY) * (y - circleY)) < radius * radius;
        }
    }
}
