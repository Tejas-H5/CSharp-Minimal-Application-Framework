﻿using OpenTK.Mathematics;
using System;
using System.Drawing;

namespace MinimalAF.Util
{
    public static class MathUtilF
    {
        public static float Snap(float x, float snapVal)
        {
            return MathF.Floor(x / snapVal) * snapVal;
        }

        public static float SnapRounded(float x, float snapVal)
        {
            return MathF.Round(x / snapVal) * snapVal;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float Mag(float x, float y)
        {
            return MathF.Sqrt(x * x + y * y);
        }

        public static float ToRadians(float v)
        {
            return v * MathF.PI / (float)180;
        }

		public static float ToDegrees(float v)
        {
            return v * (float)180/ MathF.PI;
        }

        public static float Clamp01(float value)
        {
            return MathF.Min(1, MathF.Max(value, 0));
        }
    }
}