namespace RenderingEngine.Logic
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public static class Input
    {
        internal static void Init(WindowInstance window)
        {
            _mouseInputManager = new MouseInputManager(window);
            _keyboardManager = new KeyboardInputManager(window);
        }

        internal static void Update()
        {
            _mouseInputManager.Update();
            _keyboardManager.Update();
        }

        public static float MouseX { get { return _mouseInputManager.MouseX; } }
        public static float MouseY { get { return _mouseInputManager.MouseY; } }

        // -------- mouseInputManager wrappers
        private static MouseInputManager _mouseInputManager;

        public static bool[] MouseButtonStates { get { return _mouseInputManager.MouseButtonStates; } }
        public static bool[] PrevMouseButtonStates { get { return _mouseInputManager.PrevMouseButtonStates; } }

        public static bool IsMouseClicked(MouseButton b)
        {
            return _mouseInputManager.IsMouseClicked(b);
        }

        public static bool IsMouseReleased(MouseButton b)
        {
            return _mouseInputManager.IsMouseReleased(b);
        }

        public static bool IsMouseDown(MouseButton b)
        {
            return _mouseInputManager.IsMouseDown(b);
        }

        public static bool IsMouseHeld(MouseButton b)
        {
            return _mouseInputManager.IsMouseHeld(b);
        }

        public static void StartMouseDrag(float x, float y)
        {
            _mouseInputManager.StartDrag(x, y);
        }

        public static bool IsMouseClickedAny { get { return _mouseInputManager.IsMouseClickedAny; } }
        public static bool IsMouseReleasedAny { get { return _mouseInputManager.IsMouseReleasedAny; } }
        public static bool IsMouseDownAny { get { return _mouseInputManager.IsMouseDownAny; } }
        public static bool HasMouseStartedDragging { get { return _mouseInputManager.StartedDragging; } }
        public static bool IsMouseDragging { get { return _mouseInputManager.IsDragging; } }
        public static bool WasMouseDragging { get { return _mouseInputManager.WasDragging; } }
        public static float DragStartX { get { return _mouseInputManager.DragStartX; } }
        public static float DragStartY { get { return _mouseInputManager.DragStartY; } }
        public static float DragDeltaX { get { return _mouseInputManager.DragDeltaX; } }
        public static float DragDeltaY { get { return _mouseInputManager.DragDeltaY; } }
        public static float DragDisplacement { get { return _mouseInputManager.DragDisplacement; } }

        public static void CancelDrag()
        {
            _mouseInputManager.CancelDrag();
        }

        // -------- Keyboard input wrappers
        private static KeyboardInputManager _keyboardManager;

        public static bool IsKeyPressed(KeyCode key)
        {
            return _keyboardManager.IsKeyPressed(key);
        }

        public static bool IsKeyReleased(KeyCode key)
        {
            return _keyboardManager.IsKeyReleased(key);
        }

        public static bool IsKeyDown(KeyCode key)
        {
            return _keyboardManager.IsKeyDown(key);
        }

        public static string CharactersPressed { get { return _keyboardManager.CharactersPressed; } }
        public static string CharactersReleased { get { return _keyboardManager.CharactersReleased; } }
        public static string CharactersDown { get { return _keyboardManager.CharactersDown; } }


        public static bool IsShiftDown { get { return _keyboardManager.IsShiftDown; } }
        public static bool IsCtrlDown { get { return _keyboardManager.IsCtrlDown; } }
        public static bool IsAltDown { get { return _keyboardManager.IsAltDown; } }


        public static bool IsShiftPressed { get { return _keyboardManager.IsShiftPressed; } }
        public static bool IsCtrlPressed { get { return _keyboardManager.IsCtrlPressed; } }
        public static bool IsAltPressed { get { return _keyboardManager.IsAltPressed; } }


        public static bool IsShiftReleased { get { return _keyboardManager.IsShiftReleased; } }
        public static bool IsCtrlReleased { get { return _keyboardManager.IsCtrlReleased; } }
        public static bool IsAltReleased { get { return _keyboardManager.IsAltReleased; } }

        public static bool IsAnyKeyDown { get { return _keyboardManager.IsAnyDown; } }
        public static bool IsAnyKeyPressed { get { return _keyboardManager.IsAnyPressed; } }
        public static bool IsAnyKeyReleased { get { return _keyboardManager.IsAnyReleased; } }
    }
}
