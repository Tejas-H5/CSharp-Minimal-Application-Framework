using System;

namespace MinimalAF {
    public partial class Element {
        internal virtual Rect GetParentRelativeRect() {
			return Parent.RelativeRect;
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
