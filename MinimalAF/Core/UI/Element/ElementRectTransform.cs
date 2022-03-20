using System;

namespace MinimalAF {
    public partial class Element {
        public virtual Rect GetParentRect() {
            if (Parent != null) {
                return Parent.ScreenRect;
            }

            throw new NotImplementedException("Parent wasn't hooked up properly. " +
                "Or you called GetParentRect on a 'Window' element. which shouldn't have any parents");
        }

        public float Width {
            get {
                return ScreenRect.Width;
            }
        }
        public float Height {
            get {
                return ScreenRect.Height;
            }
        }
        public float Left {
            get {
                return ScreenRect.Left;
            }
        }
        public float Bottom {
            get {
                return ScreenRect.Bottom;
            }
        }
        public float Right {
            get {
                return ScreenRect.Right;
            }
        }
        public float Top {
            get {
                return ScreenRect.Top;
            }
        }
    }
}
