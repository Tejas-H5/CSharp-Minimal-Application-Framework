using System;

namespace MinimalAF {
    public partial class Element {
        internal virtual Rect GetParentScreenRect() {
			return Parent.ScreenRect;
        }

        public float Width {
            get {
                return RelativeRect.Width;
            }
        }
        public float Height {
            get {
                return RelativeRect.Height;
            }
        }
    }
}
