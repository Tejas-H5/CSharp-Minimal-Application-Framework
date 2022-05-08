using System;
using System.Collections.Generic;

namespace MinimalAF {
    public enum Direction {
        Up, Down, Left, Right
    }

    public partial class Element {

        private float LayoutOffsetsGetOffset(int index, ArraySlice<Element> elements, float[] offsets, Direction layoutDirection, bool normalized) {
            float offset;

            if (offsets != null) {
                offset = offsets[index];
            } else {
                offset = index / ((float)elements.Length);
                normalized = true;
            }

            if (layoutDirection == Direction.Left || layoutDirection == Direction.Down) {
                offset = -offset;
            }

            if (normalized) {
                if (layoutDirection == Direction.Left || layoutDirection == Direction.Right) {
                    offset = VW(offset);
                } else {
                    offset = VH(offset);
                }
            }

            return offset;
        }

        protected void LayoutTwoSplit(Element el0, Element el1, Direction layoutDirection, float splitAmount) {
            Rect wanted0 = el0.RelativeRect;
            Rect wanted1 = el1.RelativeRect;
            if (layoutDirection == Direction.Down) {
                wanted0.Y0 += VH(1) - splitAmount;
                wanted1.Y1 -= splitAmount;
            } else if (layoutDirection == Direction.Up) {
                wanted0.Y0 += splitAmount;
                wanted1.Y1 -= VH(1) - splitAmount;
            } else if (layoutDirection == Direction.Left) {
                wanted0.X0 += VW(1) - splitAmount;
                wanted1.X1 -= splitAmount;
            } else {
                wanted1.X0 += splitAmount;
                wanted0.X1 -= VW(1) - splitAmount;
            }

            el0.RelativeRect = wanted0;
            el1.RelativeRect = wanted1;
        }

        protected void LayoutInset(ArraySlice<Element> elements, float margin) {
            LayoutInset(elements, margin, margin, margin, margin);
        }

        protected void LayoutInset(Element element, float margin) {
            LayoutInset(element, margin, margin, margin, margin);
        }

        protected void LayoutInset(ArraySlice<Element> elements, float marginLeft, float marginBottom, float marginRight, float marginTop) {
            for (int i = 0; i < elements.Length; i++) {
                LayoutInset(elements[i], marginLeft, marginBottom, marginRight, marginTop);
            }
        }

        protected void LayoutInset(Element element, float marginLeft, float marginBottom, float marginRight, float marginTop) {
            Rect wanted = element.RelativeRect;

            wanted.X0 += marginLeft;
            wanted.Y0 += marginBottom;
            wanted.X1 -= marginRight;
            wanted.Y1 -= marginTop;

            element.RelativeRect = wanted;
        }

        protected void LayoutX0(ArraySlice<Element> elements, float val) {
            foreach (var (i, el) in elements) {
                var wanted = el.RelativeRect;
                wanted.X0 = val;
                el.RelativeRect = wanted;
            }
        }

        protected void LayoutX0X1(ArraySlice<Element> elements, float x0, float x1) {
            foreach (var (i, el) in elements) {
                var wanted = el.RelativeRect;
                wanted.X0 = x0;
                wanted.X1 = x1;
                el.RelativeRect = wanted;
            }
        }

        protected void LayoutY0Y1(ArraySlice<Element> elements, float y0, float y1) {
            foreach (var (i, el) in elements) {
                var wanted = el.RelativeRect;
                wanted.Y0 = y0;
                wanted.Y1 = y1;
                el.RelativeRect = wanted;
            }
        }

        protected void LayoutX1(ArraySlice<Element> elements, float val) {
            foreach (var (i, el) in elements) {
                var wanted = el.RelativeRect;
                wanted.X1 = val;
                el.RelativeRect = wanted;
            }
        }

        protected void LayoutY0(ArraySlice<Element> elements, float val) {
            foreach (var (i, el) in elements) {
                var wanted = el.RelativeRect;
                wanted.Y0 = val;
                el.RelativeRect = wanted;
            }
        }

        protected void LayoutY1(ArraySlice<Element> elements, float val) {
            foreach (var (i, el) in elements) {
                var wanted = el.RelativeRect;
                wanted.Y1 = val;
                el.RelativeRect = wanted;
            }
        }


        protected enum AspectRatioMethod {
            DriveWidth, DriveHeight, FitInside, FitOutside
        }

        protected void LayoutAspectRatio(ArraySlice<Element> elements, float widthOverHeight, AspectRatioMethod method) {
            for (int i = 0; i < elements.Length; i++) {
                LayoutAspectRatio(elements[i], widthOverHeight, method);
            }
        }

        protected void LayoutAspectRatio(Element element, float widthOverHeight, AspectRatioMethod method) {
            bool shouldDriveHeight = false;
            Rect wanted = element.RelativeRect;

            if (method == AspectRatioMethod.DriveHeight) {
                shouldDriveHeight = true;
            } else if (method == AspectRatioMethod.DriveHeight) {
                shouldDriveHeight = true;
            } else if (method == AspectRatioMethod.FitInside || method == AspectRatioMethod.FitOutside) {
                float newWidth = wanted.Height * widthOverHeight;
                shouldDriveHeight = newWidth > wanted.Width;

                if (method == AspectRatioMethod.FitOutside) {
                    shouldDriveHeight = !shouldDriveHeight;
                }
            }

            if (shouldDriveHeight) {
                wanted = wanted.ResizedHeight(wanted.Width / widthOverHeight, _pivot.Y);
            } else {
                wanted = wanted.ResizedWidth(wanted.Height * widthOverHeight, _pivot.X);
            }

            element.RelativeRect = wanted;
        }



        /// <summary>
        /// Lays out the elements in a particular direction, and then returns
        /// width/height of the operation, which can be used
        /// to fit to the content for example.
        /// 
        /// Use <see cref="LayoutOffsets"/> if you need control on each individual size.
        /// 
        /// </summary>
        protected float LayoutLinear(ArraySlice<Element> elements, Direction layoutDirection, float elementSizing = -1, float scrollOffset = 0, float gap = 0) {
            bool vertical = layoutDirection == Direction.Up || layoutDirection == Direction.Down;
            bool reverse = layoutDirection == Direction.Down || layoutDirection == Direction.Left;

            float dir = reverse ? -1 : 1;
            float previousEnd = scrollOffset + gap * dir;

            foreach (var (index, e) in elements) {
                float end;
                if(elementSizing < 0) {
                    e.Layout();

                    end = previousEnd + e.RelativeRect.Height * dir;
                } else {
                    end = previousEnd + elementSizing * dir;
                }

                switch (layoutDirection) {
                    case Direction.Up: e.Pivot.Y = 0f; break;
                    case Direction.Down: e.Pivot.Y = 1f; break;
                    case Direction.Right: e.Pivot.X = 0f; break;
                    case Direction.Left: e.Pivot.X = 1f; break;
                };

                Rect wanted = e.RelativeRect;
                if (layoutDirection == Direction.Left) {
                    wanted.X1 = previousEnd;
                    wanted.X0 = end;
                } else if (layoutDirection == Direction.Right) {
                    wanted.X0 = previousEnd;
                    wanted.X1 = end;
                } else if (layoutDirection == Direction.Up) {
                    wanted.Y0 = previousEnd;
                    wanted.Y1 = end;
                } else {
                    wanted.Y1 = previousEnd;
                    wanted.Y0 = end;
                }

                e.RelativeRect = wanted;

                // TODO: Set elementSizing=-1 to call Layout on the child to figure out it's size and place accordingly
                previousEnd = end + gap * dir;
            }

            return MathF.Abs(scrollOffset - previousEnd);
        }
    }
}
