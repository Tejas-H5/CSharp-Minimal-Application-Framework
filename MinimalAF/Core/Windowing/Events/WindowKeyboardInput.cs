namespace MinimalAF {
    public class WindowKeyboardInput {
        Element currentlyFocused = null;

        public void FocusElement(Element el) {
            currentlyFocused = el;
        }

        public bool IsFocused(Element el) {
            return el == currentlyFocused;
        }
    }
}
