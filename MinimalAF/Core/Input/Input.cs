namespace MinimalAF
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public static class Input
    {
        private static MouseInputManager _mouseInputManager;
        private static KeyboardInputManager _keyboardManager;

        static Input()
        {
            _mouseInputManager = new MouseInputManager();
            _keyboardManager = new KeyboardInputManager();
        }

        internal static void HookToWindow(OpenTKWindowWrapper window)
        {
            _mouseInputManager.Unhook();
            _keyboardManager.Unhook();

            _mouseInputManager.Hook(window);
            _keyboardManager.Hook(window);
        }

        internal static void Update()
        {
            _mouseInputManager.Update();
            _keyboardManager.Update();
        }

        public static MouseInputManager Mouse {get{ return _mouseInputManager; }}
        public static KeyboardInputManager Keyboard {get{ return _keyboardManager; }}
    }
}
