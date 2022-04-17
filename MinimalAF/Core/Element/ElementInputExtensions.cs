using MinimalAF.Rendering;

namespace MinimalAF {
    public partial class Element {
        public float MouseX {
            get {
                return Input.Mouse.X - _screenRect.X0;
            }
        }

        public float MouseY {
            get {
                return Input.Mouse.Y - _screenRect.Y0;
            }
        }

        public float MouseXDelta => Input.Mouse.XDelta;
        public float MouseYDelta => Input.Mouse.YDelta;
        public float MouseDragStartX => Input.Mouse.DragStartX;
        public float MouseDragStartY => Input.Mouse.DragStartY;
        public float MouseDragDeltaX => Input.Mouse.DragDeltaX;
        public float MouseDragDeltaY => Input.Mouse.DragDeltaY;
        public bool MouseIsDragging => MouseOverSelf && Input.Mouse.IsDragging;
        public bool MouseStartedDragging => MouseOverSelf && Input.Mouse.StartedDragging;
        public bool MouseFinishedDragging => Input.Mouse.FinishedDragging;

        public bool MouseButtonPressed(MouseButton b) {
            return MouseOverSelf && Input.Mouse.ButtonPressed(b);
        }

        public bool MouseButtonReleased(MouseButton b) {
            return MouseOverSelf && Input.Mouse.ButtonReleased(b);
        }

        public bool MouseButtonHeld(MouseButton b) {
            return MouseOverSelf && Input.Mouse.ButtonHeld(b);
        }

        public bool MouseOverSelf => Input.Mouse.IsOver(_screenRect);

        public bool MouseOver(Rect r) {
            return MouseOver(r.X0, r.Y0, r.X1, r.Y1);
        }

        public bool MouseOver(float x0, float y0, float x1, float y1) {
            return PointOver(MouseX, MouseY, x0, y0, x1, y1);
        }

        public bool PointOver(float px, float py, float x0, float y0, float x1, float y1) {
            return Intersections.IsInsideRect(px, py, x0, y0, x1, y1);
        }

        public void CancelDrag() {
            Input.Mouse.CancelDrag();
        }

        public float MousewheelNotches {
            get {
                if (!MouseOverSelf)
                    return 0;

                return Input.Mouse.WheelNotches;
            }
        }

        public bool KeyPressed(KeyCode key) {
            return Input.Keyboard.IsPressed(key);
        }

        public bool KeyReleased(KeyCode key) {
            return Input.Keyboard.IsReleased(key);
        }

        public bool KeyHeld(KeyCode key) {
            return Input.Keyboard.IsHeld(key);
        }

        public string KeyboardCharactersTyped => Input.Keyboard.CharactersTyped;
        public string KeyboardCharactersPressed => Input.Keyboard.CharactersPressed;
        public string KeyboardCharactersReleased => Input.Keyboard.CharactersReleased;
        public string KeyboardCharactersHeld => Input.Keyboard.CharactersHeld;
    }
}
