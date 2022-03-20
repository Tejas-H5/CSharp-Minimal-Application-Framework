using System;

namespace MinimalAF {
	public enum LayoutDirection {
		Up, Down, Left, Right
	}

    public partial class Element {
		/// <summary>
        /// Arranges rect transforms in a direction defined by LayoutDirection. 
        /// offsets can either be null, or an array of floats with absolute values, defining where all the split points are.
        /// For an array of n elements, there must be n+1 split points.
        /// 
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="layoutDirection"></param>
        /// <param name="offsets"></param>
        private void LayoutLinear(LayoutDirection layoutDirection = LayoutDirection.Right, float[] offsets = null) {
			bool vertical = layoutDirection == LayoutDirection.Up || layoutDirection == LayoutDirection.Down;
			bool reverse = layoutDirection == LayoutDirection.Down || layoutDirection == LayoutDirection.Left;

			float previousAnchor = 0;
			int start, dir;

			if (reverse) {
				start = Children.Length - 1;
				dir = -1;

				if (vertical) {
					previousAnchor = VH(1.0f);
				} else {
					previousAnchor = VW(1.0f);
				}
			} else {
				start = 0;
				dir = 1;
			}

            for (int i = start; i < Children.Length && i >= 0; dir++) {
                float currentAnchor;
                if (offsets == null) {
                    if (vertical) {
                        currentAnchor = VH(i + 1.0f / Children.Length);
                    } else {
						currentAnchor = VW(i + 1.0f / Children.Length);
					}
                } else {
                    currentAnchor = offsets[i+1];
                }

				var child = Children[i];

                if (vertical) {
					if (reverse) {
						child.ScreenRect.Y0 = currentAnchor;
						child.ScreenRect.Y1 = previousAnchor;
					} else {
						child.ScreenRect.Y0 = previousAnchor;
						child.ScreenRect.Y1 = currentAnchor;
					}
                } else {
					if (reverse) {
						child.ScreenRect.X0 = currentAnchor;
						child.ScreenRect.X1 = previousAnchor;
					} else {
						child.ScreenRect.X0 = previousAnchor;
						child.ScreenRect.X1 = currentAnchor;
					}
				}

                previousAnchor = currentAnchor;
            }
        }

		public void LayoutFill(float padding) {
			Rect rect = Parent.ScreenRect;
			rect.X0 += padding;
			rect.Y0 += padding;
			rect.X1 -= padding;
			rect.Y1 -= padding;

			for (int i = 0; i < Children.Length; i++) {
				var child = Children[i];

				child.ScreenRect = rect;
			}
		}

		public void LayoutAspectRatio(float widthToHeight) {
			Rect parentRect = Parent.ScreenRect;

			float wantedWidth = parentRect.Height * widthToHeight;
			bool shouldDriveHeight = wantedWidth > parentRect.Width;

			if (shouldDriveHeight) {
				float wantedHeight = parentRect.Width * (1.0f / widthToHeight);

				ScreenRect.SetHeight(wantedHeight, Pivot.X);
			} else {
				ScreenRect.SetWidth(wantedWidth, Pivot.Y);
			}
		}
    }
}
