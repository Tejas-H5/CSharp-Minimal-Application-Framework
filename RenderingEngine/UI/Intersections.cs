using RenderingEngine.Datatypes.Geometric;
using System.Drawing;

namespace RenderingEngine.UI
{
    public static class Intersections
    {
        public static bool IsInsideRect(PointF p, Rect2D a)
        {
            return IsInsideRect(p.X, p.Y, a.Left, a.Bottom, a.Right, a.Top);
        }

        public static bool IsInsideRect(float x, float y, Rect2D a)
        {
            return IsInsideRect(x, y, a.Left, a.Bottom, a.Right, a.Top);
        }

        public static bool IsInsideRect(float x, float y, float left, float bottom, float right, float top)
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
