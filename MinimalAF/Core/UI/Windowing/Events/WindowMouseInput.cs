namespace MinimalAF {
    public class WindowMouseInput : IWindowResource {
        /// <summary>
        /// Has the mouse been clicked anywhere?
        /// 
        /// Resets every Update frame
        /// </summary>
        public bool ClickHandled = false;

        /// <summary>
        /// Has the mouse hover event been handled?
        /// 
        /// Resets every Update frame
        /// </summary>
        public bool HoverHandled = false;

        public void Update() {
            ClickHandled = false;
            HoverHandled = false;
        }

        /// <summary>
        /// Checks if the mouse is hovering over this element only if the hover event hasn't already been handled.
        /// Will set HoverHandled to true automatically
        /// </summary>
        public bool CheckAndHandleOver(Element element) {
            if (HoverHandled)
                return false;

            if (Input.Mouse.IsOver(element.RectTransform.Rect)) {
                HoverHandled = true;
                return true;
            }


            return false;
        }

        /// <summary>
        /// Checks if the mouse has been pressed this frame, anywhere. To be used with <see cref="CheckAndHandleOver(Element)"/> 
        /// 
        /// Returns false if ClickHandled.
        /// </summary>
        public bool CheckAndHandlePressed() {
            if (ClickHandled)
                return false;

            if (Input.Mouse.IsAnyPressed) {
                ClickHandled = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the mouse has been held down, anywhere. To be used with <see cref="CheckAndHandleOver(Element)"/> 
        /// 
        /// Returns false if ClickHandled.
        /// </summary>
        public bool CheckAndHandleDown() {
            if (ClickHandled)
                return false;

            if (Input.Mouse.IsAnyDown) {
                ClickHandled = true;
                return true;
            }

            return false;
        }
    }
}
