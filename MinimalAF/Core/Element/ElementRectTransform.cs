using System;

namespace MinimalAF {
    public partial class Element {
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
