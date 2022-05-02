using MinimalAF.Rendering;

namespace MinimalAF {
    public partial class Element {
        protected float MouseX {
            get {
                return Input.Mouse.X - screenRect.X0;
            }
        }

        protected float MouseY {
            get {
                return Input.Mouse.Y - screenRect.Y0;
            }
        }

        protected float MouseXDelta => Input.Mouse.XDelta;
        protected float MouseYDelta => Input.Mouse.YDelta;
        protected float MouseDragStartX => Input.Mouse.DragStartX;
        protected float MouseDragStartY => Input.Mouse.DragStartY;
        protected float MouseDragDeltaX => Input.Mouse.DragDeltaX;
        protected float MouseDragDeltaY => Input.Mouse.DragDeltaY;
        protected bool MouseIsDragging => MouseOverSelf && Input.Mouse.IsDragging;
        protected bool MouseStartedDragging => MouseOverSelf && Input.Mouse.StartedDragging;
        protected bool MouseFinishedDragging => Input.Mouse.FinishedDragging;

        protected bool MouseButtonPressed(MouseButton b) {
            return MouseOverSelf && Input.Mouse.ButtonPressed(b);
        }

        protected bool MouseButtonReleased(MouseButton b) {
            return MouseOverSelf && Input.Mouse.ButtonReleased(b);
        }

        protected bool MouseButtonHeld(MouseButton b) {
            return MouseOverSelf && Input.Mouse.ButtonHeld(b);
        }

        protected bool MouseOverSelf => Input.Mouse.IsOver(screenRect);

        protected bool MouseOver(Rect r) {
            return MouseOver(r.X0, r.Y0, r.X1, r.Y1);
        }

        protected bool MouseOver(float x0, float y0, float x1, float y1) {
            return PointOver(MouseX, MouseY, x0, y0, x1, y1);
        }

        protected bool PointOver(float px, float py, float x0, float y0, float x1, float y1) {
            return Intersections.IsInsideRect(px, py, x0, y0, x1, y1);
        }

        protected void CancelDrag() {
            Input.Mouse.CancelDrag();
        }

        protected float MousewheelNotches {
            get {
                if (!MouseOverSelf)
                    return 0;

                return Input.Mouse.WheelNotches;
            }
        }

        protected bool KeyPressed(KeyCode key) {
            return Input.Keyboard.IsPressed(key);
        }

        protected bool KeyReleased(KeyCode key) {
            return Input.Keyboard.IsReleased(key);
        }

        protected bool KeyHeld(KeyCode key) {
            return Input.Keyboard.IsHeld(key);
        }

        protected string KeyboardCharactersTyped => Input.Keyboard.CharactersTyped;
        protected string KeyboardCharactersPressed => Input.Keyboard.CharactersPressed;
        protected string KeyboardCharactersReleased => Input.Keyboard.CharactersReleased;
        protected string KeyboardCharactersHeld => Input.Keyboard.CharactersHeld;
    }
}
