using System;
using System.Collections.Generic;

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
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="layoutDirection"></param>
		/// <param name="offsets"></param>
		public void LayoutElementsLinear(ReadOnlySpan<Element> elements, LayoutDirection layoutDirection = LayoutDirection.Right, float[] offsets = null, bool normalizedOffsets = false) {
			bool vertical = layoutDirection == LayoutDirection.Up || layoutDirection == LayoutDirection.Down;
			bool reverse = layoutDirection == LayoutDirection.Down || layoutDirection == LayoutDirection.Left;

			if (offsets == null) {
				normalizedOffsets = false;
			}

			float previousAnchor;

			if (offsets != null) {
				previousAnchor = offsets[0];
			} else {
				previousAnchor = 0;
			}

			if (reverse) {
				if (vertical) {
					if (normalizedOffsets) {
						previousAnchor = VH(previousAnchor);
					}

					previousAnchor = VH(1.0f) - previousAnchor;
				} else {
					if (normalizedOffsets) {
						previousAnchor = VW(previousAnchor);
					}

					previousAnchor = VW(1.0f) - previousAnchor;
				}
			}

			int i = 0;
			foreach (var el in elements) {
				float currentAnchor;
				if (offsets == null) {
					if (vertical) {
						currentAnchor = VH(i + 1.0f / elements.Length);
					} else {
						currentAnchor = VW(i + 1.0f / elements.Length);
					}
				} else {
					currentAnchor = offsets[i + 1];
				}

				if (vertical) {
					if (normalizedOffsets) {
						currentAnchor = VW(currentAnchor);
					}

					if (reverse) {
						el.RelativeRect.Y0 = currentAnchor;
						el.RelativeRect.Y1 = previousAnchor;
					} else {
						el.RelativeRect.Y0 = previousAnchor;
						el.RelativeRect.Y1 = currentAnchor;
					}
				} else {
					if (normalizedOffsets) {
						currentAnchor = VH(currentAnchor);
					}

					if (reverse) {
						el.RelativeRect.X0 = currentAnchor;
						el.RelativeRect.X1 = previousAnchor;
					} else {
						el.RelativeRect.X0 = previousAnchor;
						el.RelativeRect.X1 = currentAnchor;
					}
				}

				previousAnchor = currentAnchor;

				i++;
			}
		}

		public void LayoutElementsSplit(ReadOnlySpan<Element> elements, LayoutDirection layoutDirection, float splitAmount) {
			if (elements.Length != 2) {
				throw new Exception("Only 2 elements may be involved in a split");
			}
		}

		/// <summary>
        /// 
        /// </summary>
        /// <param name="el0"></param>
        /// <param name="el1"></param>
        /// <param name="layoutDirection"></param>
        /// <param name="splitAmount"></param>
		public void LayoutElementsSplit(Element el0, Element el1, LayoutDirection layoutDirection, float splitAmount) {
			if (layoutDirection == LayoutDirection.Down) {
				el0.RelativeRect.Y0 += VH(1) - splitAmount;
				el1.RelativeRect.Y1 -= splitAmount;
			} else if (layoutDirection == LayoutDirection.Up) {
				el0.RelativeRect.Y0 += splitAmount;
				el1.RelativeRect.Y1 -= VH(1) - splitAmount;
			} else if (layoutDirection == LayoutDirection.Left) {
				el0.RelativeRect.X0 += VW(1) - splitAmount;
				el1.RelativeRect.X1 -= splitAmount;
			} else {
				el0.RelativeRect.X0 += splitAmount;
				el1.RelativeRect.X1 -= VW(1) - splitAmount;
			}
		}

		public void LayoutRelativeMargin(float margin) {
			RelativeRect.X0 += margin;
			RelativeRect.Y0 += margin;
			RelativeRect.X1 -= margin;
			RelativeRect.Y1 -= margin;
		}

		public void LayoutMargin(float margin) {
			var parentRect = GetParentScreenRect();
			RelativeRect = new Rect(0, 0, parentRect.X1, parentRect.Y1);

			LayoutRelativeMargin(margin);
		}

		public void LayoutMargin(float marginLeft, float marginBottom, float marginRight, float marginTop) {

		}

		public void LayoutRelativeMargin(float marginLeft, float marginBottom, float marginRight, float marginTop) {
			RelativeRect.X0 += marginLeft;
			RelativeRect.Y0 += marginBottom;
			RelativeRect.X1 -= marginRight;
			RelativeRect.Y1 -= marginTop;
		}

		public enum AspectRatioMethod {
			DriveWidth, DriveHeight, FitInside, FitOutside
		}

		public void LayoutAspectRatio(float widthOverHeight, AspectRatioMethod method) {
			bool shouldDriveHeight = false;

			if (method == AspectRatioMethod.DriveHeight) {
				shouldDriveHeight = true;
			} else if (method == AspectRatioMethod.DriveHeight) {
				RelativeRect.SetHeight(RelativeRect.Width / widthOverHeight);
			} else if (method == AspectRatioMethod.FitInside || method == AspectRatioMethod.FitOutside) {
				float newWidth = RelativeRect.Height * widthOverHeight;
				shouldDriveHeight = newWidth > Parent.RelativeRect.Width;

				if (method == AspectRatioMethod.FitOutside) {
					shouldDriveHeight = !shouldDriveHeight;
				}
			}

			if (shouldDriveHeight) {
				RelativeRect.SetHeight(RelativeRect.Width / widthOverHeight);
			} else {
				RelativeRect.SetWidth(RelativeRect.Height * widthOverHeight);
			}
		}
	}
}
