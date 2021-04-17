using RenderingEngine.Datatypes.Geometric;
using System.Drawing;

namespace RenderingEngine.UI
{
    public static class Intersections
    {
        public static bool IsInside(PointF p, Rect2D a)
        {
            return IsInside(p.X, p.Y, a.Left, a.Bottom, a.Right, a.Top);
        }

        public static bool IsInside(float x, float y, Rect2D a)
        {
            return IsInside(x, y, a.Left, a.Bottom, a.Right, a.Top);
        }

        public static bool IsInside(float x, float y, float left, float bottom, float right, float top)
        {
            if (x >= left && x <= right)
            {
                if (y <= top && y >= bottom)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInsideCircle(float x, float y, float circleX, float circleY, float radius)
        {
            return ((x - circleX) * (x - circleX) + (y - circleY) * (y - circleY)) < (radius * radius);
        }
    }
}
