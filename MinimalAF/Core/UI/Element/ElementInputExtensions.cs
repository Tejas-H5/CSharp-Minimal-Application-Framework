using MinimalAF.Rendering;

namespace MinimalAF
{
	public partial class Element
	{
		public float MouseX {
			get {
				return Input.Mouse.X - CTX.CurrentScreenRect.X0;
			}
		}

		public float MouseY {
			get {
				return Input.Mouse.Y - CTX.CurrentScreenRect.Y0;
			}
		}

		public float MouseXDelta => Input.Mouse.XDelta;
		public float MouseYDelta => Input.Mouse.YDelta;
		public float MouseDragStartX => Input.Mouse.DragStartX;
		public float MouseDragStartY => Input.Mouse.DragStartY;
		public float MouseDragDeltaX => Input.Mouse.DragDeltaX;
		public float MouseDragDeltaY => Input.Mouse.DragDeltaY;
		public bool MouseIsDragging => Input.Mouse.IsDragging;
		public bool MouseStartedDragging => Input.Mouse.StartedDragging;
		public bool MouseFinishedDragging => Input.Mouse.FinishedDragging;

		public bool MouseButtonIsPressed(MouseButton b) {
			return Input.Mouse.IsPressed(b);
		}

		public bool MouseButtonIsReleased(MouseButton b) {
			return Input.Mouse.IsReleased(b);
		}

		public bool MouseButtonIsDown(MouseButton b) {
			return Input.Mouse.IsDown(b);
		}

		public bool MouseOverSelf() {
			return Input.Mouse.IsOver(RectTransform.Rect);
		}

		public void CancelDrag() {
			Input.Mouse.CancelDrag();
		}

		public float MouseWheelNotches => Input.Mouse.WheelNotches;

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
