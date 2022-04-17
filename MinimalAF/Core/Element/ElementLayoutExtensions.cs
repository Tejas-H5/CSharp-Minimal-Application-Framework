using System;
using System.Collections.Generic;

namespace MinimalAF {
    public enum Direction {
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
        public void LayoutSplit(ArraySlice<Element> elements, Direction layoutDirection = Direction.Right, float[] offsets = null, bool normalizedOffsets = false) {
            bool vertical = layoutDirection == Direction.Up || layoutDirection == Direction.Down;
            bool reverse = layoutDirection == Direction.Down || layoutDirection == Direction.Left;

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
                        currentAnchor = VH((i + 1.0f) / elements.Length);

                        if (reverse) {
                            currentAnchor = VH(1) - currentAnchor;
                        }
                    } else {
                        currentAnchor = VW((i + 1.0f) / elements.Length);

                        if (reverse) {
                            currentAnchor = VW(1) - currentAnchor;
                        }
                    }
                } else {
                    currentAnchor = offsets[i + 1];

                    if (normalizedOffsets) {
                        if (vertical) {
                            currentAnchor = VH(currentAnchor);
                        } else {
                            currentAnchor = VW(currentAnchor);
                        }
                    }

                    if (reverse) {
                        if (vertical) {
                            currentAnchor = VH(1) - currentAnchor;
                        } else {
                            currentAnchor = VW(1) - currentAnchor;
                        }
                    }
                }

                Rect wanted = el.RelativeRect;

                if (vertical) {
                    if (reverse) {
                        wanted.Y0 = currentAnchor;
                        wanted.Y1 = previousAnchor;
                    } else {
                        wanted.Y0 = previousAnchor;
                        wanted.Y1 = currentAnchor;
                    }
                } else {
                    if (reverse) {
                        wanted.X0 = currentAnchor;
                        wanted.X1 = previousAnchor;
                    } else {
                        wanted.X0 = previousAnchor;
                        wanted.X1 = currentAnchor;
                    }
                }

                el.RelativeRect = wanted;

                previousAnchor = currentAnchor;

                i++;
            }
        }

        public void LayoutTwoSplit(Element el0, Element el1, Direction layoutDirection, float splitAmount) {
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

        public void LayoutInset(ArraySlice<Element> elements, float margin) {
            LayoutInset(elements, margin, margin, margin, margin);
        }

        public void LayoutInset(Element element, float margin) {
            LayoutInset(element, margin, margin, margin, margin);
        }

        public void LayoutInset(ArraySlice<Element> elements, float marginLeft, float marginBottom, float marginRight, float marginTop) {
            for (int i = 0; i < elements.Length; i++) {
                LayoutInset(elements[i], marginLeft, marginBottom, marginRight, marginTop);
            }
        }

        public void LayoutInset(Element element, float marginLeft, float marginBottom, float marginRight, float marginTop) {
            Rect wanted = element.RelativeRect;

            wanted.X0 += marginLeft;
            wanted.Y0 += marginBottom;
            wanted.X1 -= marginRight;
            wanted.Y1 -= marginTop;

            element.RelativeRect = wanted;
        }

        public enum AspectRatioMethod {
            DriveWidth, DriveHeight, FitInside, FitOutside
        }

        public void LayoutAspectRatio(ArraySlice<Element> elements, float widthOverHeight, AspectRatioMethod method) {
            for (int i = 0; i < elements.Length; i++) {
                LayoutAspectRatio(elements[i], widthOverHeight, method);
            }
        }

        public void LayoutAspectRatio(Element element, float widthOverHeight, AspectRatioMethod method) {
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
                wanted.SetHeight(wanted.Width / widthOverHeight, Pivot.Y);
            } else {
                wanted.SetWidth(wanted.Height * widthOverHeight, Pivot.X);
            }

            element.RelativeRect = wanted;
        }


        /// <summary>
        /// Use <see cref="LayoutSplit"/> if you need control on each individual size.
        /// 
        /// 
        /// </summary>
        public void LayoutLinear(ArraySlice<Element> elements, Direction layoutDirection, float elementSizing, float scrollOffset = 0, float gap = 0) {
            bool vertical = layoutDirection == Direction.Up || layoutDirection == Direction.Down;
            bool reverse = layoutDirection == Direction.Down || layoutDirection == Direction.Left;

            float dir;
            float previousEnd = scrollOffset;
            if(reverse) {
                dir = -1;

                if (vertical) {
                    previousEnd = VH(1) - scrollOffset;
                } else {
                    previousEnd = VW(1) - scrollOffset;
                }
            } else {
                dir = 1;
            }


            foreach (Element e in elements) {
                float end = previousEnd + elementSizing * dir;

                Rect wanted = e.RelativeRect;
                if(layoutDirection == Direction.Left) {
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
        }
    }
}
