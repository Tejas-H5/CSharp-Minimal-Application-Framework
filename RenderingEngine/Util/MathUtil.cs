using OpenTK.Mathematics;
using System;
using System.Drawing;

namespace RenderingEngine.Util
{

    public static class MathUtil
    {
        public static float Snap(float x, float snapVal)
        {
            return MathF.Floor(x / snapVal) * snapVal;
        }

        public static double Snap(double x, double snapVal)
        {
            return Math.Floor(x / snapVal) * snapVal;
        }

        public static float SnapRounded(float x, float snapVal)
        {
            return MathF.Round(x / snapVal) * snapVal;
        }

        public static double SnapRounded(double x, double snapVal)
        {
            return Math.Round(x / snapVal) * snapVal;
        }

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

        public static PointF Times(PointF a, float s)
        {
            return new PointF(a.X * s, a.Y * s);
        }

        public static PointF Rotate(PointF a, PointF center, float angle)
        {
            /*
            PointF vecCenterToA = Sub(a, center);
            PointF rotated = new PointF(MathF.Cos(angle) * vecCenterToA.X, MathF.Sin(angle) * vecCenterToA.Y);
            return Add(center, rotated);
            */

            float x = (a.X - center.X);
            float y = (a.Y - center.Y);

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            float xR = x * cos - y * sin;
            float yR = y * cos + x * sin;

            return new PointF(center.X + xR, center.Y + yR);
        }

        public static Matrix4 Translation(float x, float y, float z = 0)
        {
            return Matrix4.Transpose(Matrix4.CreateTranslation(x, y, z));
        }

        public static Matrix4 Rotation2D(float zAngles)
        {
            return Matrix4.CreateRotationZ(zAngles);
        }

        public static Matrix4 Rotation(float x, float y, float z)
        {
            return Matrix4.CreateRotationX(x) * Matrix4.CreateRotationY(y) * Matrix4.CreateRotationY(z);
        }

        public static Matrix4 Scale(float sf)
        {
            return Matrix4.CreateScale(sf);
        }

        public static Matrix4 Scale(float x, float y, float z)
        {
            return Matrix4.CreateScale(x, y, z);
        }

        public static float Clamp01(float value)
        {
            return MathF.Min(1, MathF.Max(value, 0));
        }
    }
}
