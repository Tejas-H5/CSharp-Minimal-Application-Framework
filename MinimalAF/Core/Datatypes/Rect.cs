using OpenTK.Mathematics;
using System;

namespace MinimalAF {
    public struct Rect {
        public float X0;
        public float Y0;
        public float X1;
        public float Y1;

        public Rect(float x0, float y0, float x1, float y1) {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
        }

        public static Rect TwoPoint(float x0, float y0, float x1, float y1) {
            return new Rect(x0, y0, x1, y1);
        }

        public static Rect PivotSize(float width, float height, float xPivot, float yPivot) {
            return new Rect(
                -xPivot * width,
                -yPivot * height,
                (1.0f - xPivot) * width,
                (1.0f - yPivot) * height
            );
        }


        public Rect ResizedWidth(float newWidth, float center = 0) {
            Rect newRect = this;

            float delta = Width - newWidth;
            newRect.X0 += delta * center;
            newRect.X1 -= delta * (1.0f - center);

            return newRect;
        }

        public Rect ResizedHeight(float newHeight, float center = 0) {
            Rect newRect = this;

            float delta = Height - newHeight;
            newRect.Y0 += delta * center;
            newRect.Y1 -= delta * (1.0f - center);

            return newRect;
        }

        public float Left => X0 < X1 ? X0 : X1;
        public float Right => X0 < X1 ? X1 : X0;
        public float Bottom => Y0 < Y1 ? Y0 : Y1;
        public float Top => Y0 < Y1 ? Y1 : Y0;
        public float CenterX => X0 + (X1 - X0) * 0.5f;

        // TODO: override the divide operator for this
        public Rect Normalized(float width, float height) {
            Rect newRect = this;

            newRect.X0 /= width;
            newRect.X1 /= width;
            newRect.Y0 /= height;
            newRect.Y1 /= height;

            return newRect;
        }


        public Rect Rectified() {
            var newRect = new Rect(X0, Y0, X1, Y1);

            if (X0 > X1) {
                float temp = X1;
                newRect.X1 = X0;
                newRect.X0 = temp;
            }

            if (Y0 > Y1) {
                float temp = Y1;
                newRect.Y1 = Y0;
                newRect.Y0 = temp;
            }

            return newRect;
        }

        public float CenterY => Y0 + (Y1 - Y0) * 0.5f;
        public float SmallerDimension => Width < Height ? Width : Height;
        public float LargerDimension => Width > Height ? Width : Height;
        public float Width => MathF.Abs(X0 - X1);
        public float Height => MathF.Abs(Y0 - Y1);
        
        public bool IsInverted() {
            return X0 > X1 || Y0 > Y1;
        }

        public override bool Equals(object obj) {
            return obj is Rect d &&
                   X0 == d.X0 &&
                   Y0 == d.Y0 &&
                   X1 == d.X1 &&
                   Y1 == d.Y1;
        }

        public override int GetHashCode() {
            return HashCode.Combine(X0, Y0, X1, Y1);
        }

        public static bool operator ==(Rect left, Rect right) {
            return left.Equals(right);
        }

        public static bool operator !=(Rect left, Rect right) {
            return !(left == right);
        }

        public override string ToString() {
            return "{" + X0 + ", " + Y0 + ", " + X1 + ", " + Y1 + "}";
        }

        public Rect Moved(float x, float y) {
            Rect newRect = this;
            newRect.X0 += x;
            newRect.X1 += x;
            newRect.Y0 += y;
            newRect.Y1 += y;
            return newRect;
        }

        /// <summary>
        /// Returns the union intersect of two rectangles. 
        /// 
        /// (a smaller rectangle corresponding to the area where two rectangles are overlapping)
        /// </summary>
        public Rect Intersected(Rect other) {
            return new Rect(
                MathF.Max(Left, other.Left),
                MathF.Max(Bottom, other.Bottom),
                MathF.Min(Right, other.Right),
                MathF.Min(Top, other.Top)
            );
        }

        public Rect Inset(float margin) {
            return Inset(margin, margin, margin, margin);
        }

        public Rect Inset(float left, float bottom, float right, float top) {
            return new Rect(X0 + left, Y0 + bottom, X1 - right, Y1 - top);
        }

        public static Rect operator *(Rect rect, float mult) {
            return new Rect(rect.X0 * mult, rect.Y0 * mult, rect.X1 * mult, rect.Y1 * mult);
        }

        public Vector2 Constrain(Vector2 point) {
            point.X = MathHelper.Clamp(point.X, X0, X1);
            point.Y = MathHelper.Clamp(point.Y, Y0, Y1);

            return point;
        }
    }
}
