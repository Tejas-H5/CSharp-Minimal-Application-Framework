using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.Util
{

    public static class MathUtil
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static PointF Lerp(PointF a, PointF b, float t)
        {
            return new PointF(
                    Lerp(a.X, b.X, t),
                    Lerp(a.Y, b.Y, t)
                );
        }

        public static float Mag(float x, float y)
        {
            return MathF.Sqrt(x * x + y * y);
        }

        public static float Mag(PointF a)
        {
            return MathF.Sqrt(a.X * a.X + a.Y * a.Y);
        }

        public static float Dot(PointF a, PointF b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static float FindAngle(PointF a, PointF b)
        {
            return MathF.Acos(Dot(a, b) / (Mag(a) * Mag(b)));
        }

        public static float FindAngle(PointF a, PointF b, PointF c)
        {
            return FindAngle(new PointF(b.X - a.X, b.Y - a.Y), new PointF(c.X - b.X, c.Y - b.Y));
        }

        public static float ToRadians(float v)
        {
            return v * MathF.PI / 180f;
        }

        public static PointF Sub(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public static PointF Add(PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }
    }
}
