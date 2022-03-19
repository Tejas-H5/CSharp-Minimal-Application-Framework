using System;

namespace MinimalAF.Util {
    public static class MathUtil {
        public static double Snap(double x, double snapVal) {
            return Math.Floor(x / snapVal) * snapVal;
        }

        public static double SnapRounded(double x, double snapVal) {
            return Math.Round(x / snapVal) * snapVal;
        }

        public static double Lerp(double a, double b, double t) {
            return a + (b - a) * t;
        }

        public static double Mag(double x, double y) {
            return Math.Sqrt(x * x + y * y);
        }

        public static double ToRadians(double v) {
            return v * Math.PI / (double)180;
        }

        public static double ToDegrees(double v) {
            return v * (double)180 / Math.PI;
        }

        public static double Clamp01(double value) {
            return Math.Min(1, Math.Max(value, 0));
        }
    }
}
