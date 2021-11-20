using System;
using System.Drawing;

namespace MinimalAF.Util
{
	public static class MathUtilPF
    {
        public static PointF Lerp(PointF a, PointF b, float t)
        {
            return new PointF(
                    MathUtilF.Lerp(a.X, b.X, t),
                    MathUtilF.Lerp(a.Y, b.Y, t)
                );
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
            float dotAB = Dot(a, b);
            float magAB = (Mag(a) * Mag(b));
            float input = MathF.Min(MathF.Max(dotAB / magAB, -1), 1);

            float res = MathF.Acos(input);
            return res;
        }

        public static float FindAngle(PointF a, PointF b, PointF c)
        {
            return FindAngle(new PointF(b.X - a.X, b.Y - a.Y), new PointF(c.X - b.X, c.Y - b.Y));
        }

        public static PointF Sub(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public static PointF Add(PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static PointF Times(PointF a, float s)
        {
            return new PointF(a.X * s, a.Y * s);
        }

        public static PointF Rotate(PointF a, PointF center, float angle)
        {
            float x = (a.X - center.X);
            float y = (a.Y - center.Y);

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            float xR = x * cos - y * sin;
            float yR = y * cos + x * sin;

            return new PointF(center.X + xR, center.Y + yR);
        }
    }
}
