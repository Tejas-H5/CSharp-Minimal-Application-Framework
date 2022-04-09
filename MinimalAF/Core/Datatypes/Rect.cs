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

		public void SetWidth(float newWidth, float center = 0) {
			float delta = Width - newWidth;
			X0 += delta * center;
			X1 -= delta * (1.0f - center);
		}

		public void SetHeight(float newHeight, float center = 0) {
			float delta = Height - newHeight;
			Y0 += delta * center;
			Y1 -= delta * (1.0f - center);
		}

		public float Left {
			get {
				return X0 < X1 ? X0 : X1;
			}
		}
		public float Right {
			get {
				return X0 < X1 ? X1 : X0;
			}
		}
		public float Bottom {
			get {
				return Y0 < Y1 ? Y0 : Y1;
			}
		}
		public float Top {
			get {
				return Y0 < Y1 ? Y1 : Y0;
			}
		}

		public float CenterX {
			get {
				return X0 + (X1 - X0) * 0.5f;
			}
		}

		public Rect Rectify() {
			if (X0 > X1) {
				float temp = X1;
				X1 = X0;
				X0 = temp;
			}

			if (Y0 > Y1) {
				float temp = Y1;
				Y1 = Y0;
				Y0 = temp;
			}

			return this;
		}

		public float CenterY {
			get {
				return Y0 + (Y1 - Y0) * 0.5f;
			}
		}

		public float SmallerDimension {
			get {
				return Width < Height ? Width : Height;
			}
		}

		public float LargerDimension {
			get {
				return Width > Height ? Width : Height;
			}
		}

		public float Width {
			get {
				return MathF.Abs(X0 - X1);
			}
		}

		public float Height {
			get {
				return MathF.Abs(Y0 - Y1);
			}
		}

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

		public void Move(float x, float y) {
			X0 += x;
			X1 += x;
			Y0 += y;
			Y1 += y;
		}

        /// <summary>
        /// Allows you to set the rect without any property side-effects.
        /// </summary>
        /// <param name="other"></param>
        public void SetPure(float x0, float y0, float x1, float y1) {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
        }

		public Rect Intersect(Rect other) {
			return new Rect(
				MathF.Max(Left, other.Left),
				MathF.Max(Bottom, other.Bottom),
				MathF.Min(Right, other.Right),
				MathF.Min(Top, other.Top)
			);
		}
	}
}
