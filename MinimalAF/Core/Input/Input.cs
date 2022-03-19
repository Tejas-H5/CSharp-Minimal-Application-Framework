namespace MinimalAF {

    internal static class Input {
        private static MouseInputManager _mouseInputManager;
        private static KeyboardInputManager _keyboardManager;

        static Input() {
            _mouseInputManager = new MouseInputManager();
            _keyboardManager = new KeyboardInputManager();
        }

        internal static void HookToWindow(OpenTKWindowWrapper window) {
            _mouseInputManager.Unhook();
            _keyboardManager.Unhook();

            _mouseInputManager.Hook(window);
            _keyboardManager.Hook(window);
        }

        internal static void Update() {
            _mouseInputManager.Update();
            _keyboardManager.Update();
        }

        internal static MouseInputManager Mouse {
            get {
                return _mouseInputManager;
            }
        }
        internal static KeyboardInputManager Keyboard {
            get {
                return _keyboardManager;
            }
        }
    }
}
