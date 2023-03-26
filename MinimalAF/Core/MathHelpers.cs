using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAF {
    public static class MathHelpers {
        public static int Min(int a, int b) {
            return a < b ? a : b;
        }

        public static int Max(int a, int b) {
            return a > b ? a : b;
        }

        public static float Min(float a, float b) {
            return a < b ? a : b;
        }

        public static float Max(float a, float b) {
            return a > b ? a : b;
        }

        public static double Min(double a, double b) {
            return a < b ? a : b;
        }

        public static double Max(double a, double b) {
            return a > b ? a : b;
        }
        public static uint Min(uint a, uint b) {
            return a < b ? a : b;
        }

        public static uint Max(uint a, uint b) {
            return a > b ? a : b;
        }

        public static float Lerp(float a, float b, float t) {
            return a + (b - a) * t;
        }

        public static int Clamp(int val, int a, int b) {
            if (val < a) return a;
            if (val > b) return b;
            return val;
        }

        public static float Clamp(float val, float a, float b) {
            if (val < a) return a;
            if (val > b) return b;
            return val;
        }

        public static double Clamp(double val, double a, double b) {
            if (val < a) return a;
            if (val > b) return b;
            return val;
        }

        public static uint Clamp(uint val, uint a, uint b) {
            if (val < a) return a;
            if (val > b) return b;
            return val;
        }

        public static double Lerp(double a, double b, double t) {
            return a + (b - a) * t;
        }

        public static float MoveTowards(float a, float b, float step) {
            if (MathF.Abs(a - b) < step) return b;

            return a > b ? a - step : a + step;
        }

        public static double MoveTowards(double a, double b, double step) {
            if (Math.Abs(a - b) < step) return b;

            return a > b ? a - step : a + step;
        }
    }
}
