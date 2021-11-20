using System;

namespace MinimalAF
{
    public struct Rect2D
    {
        public float X0;
        public float Y0;
        public float X1;
        public float Y1;

        public Rect2D(float x0, float y0, float x1, float y1)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
        }

        public float Left { get { return X0 < X1 ? X0 : X1; } }
        public float Right { get { return X0 < X1 ? X1 : X0; } }
        public float Bottom { get { return Y0 < Y1 ? Y0 : Y1; } }
        public float Top { get { return Y0 < Y1 ? Y1 : Y0; } }

        public float CenterX { get { return X0 + (X1 - X0) * 0.5f; } }

        public Rect2D Rectify()
        {
            if (X0 > X1)
            {
                float temp = X1;
                X1 = X0;
                X0 = temp;
            }

            if (Y0 > Y1)
            {
                float temp = Y1;
                Y1 = Y0;
                Y0 = temp;
            }

            return this;
        }

        public float CenterY { get { return Y0 + (Y1 - Y0) * 0.5f; } }

        public float SmallerDimension { get { return Width < Height ? Width : Height; } }

        public float LargerDimension { get { return Width > Height ? Width : Height; } }

        public float Width {
            get {
                return Right - Left;
            }
        }

        public float Height {
            get {
                return Top - Bottom;
            }
        }

        public bool IsInverted()
        {
            return X0 > X1 || Y0 > Y1;
        }

        public override bool Equals(object obj)
        {
            return obj is Rect2D d &&
                   X0 == d.X0 &&
                   Y0 == d.Y0 &&
                   X1 == d.X1 &&
                   Y1 == d.Y1;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X0, Y0, X1, Y1);
        }

        public static bool operator ==(Rect2D left, Rect2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rect2D left, Rect2D right)
        {
            return !(left == right);
        }

        public Rect2D Intersect(Rect2D other)
        {
            return new Rect2D(
                MathF.Max(Left, other.Left),
                MathF.Max(Bottom, other.Bottom),
                MathF.Min(Right, other.Right),
                MathF.Min(Top, other.Top)
            );
        }
    }
}
