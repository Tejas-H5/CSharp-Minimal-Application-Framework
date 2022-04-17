namespace MinimalAF {

    public static class Input {
        private static MouseInputManager mouseInputManager;
        private static KeyboardInputManager keyboardManager;

        static Input() {
            mouseInputManager = new MouseInputManager();
            keyboardManager = new KeyboardInputManager();
        }

        internal static void HookToWindow(OpenTKWindowWrapper window) {
            mouseInputManager.Unhook();
            keyboardManager.Unhook();

            mouseInputManager.Hook(window);
            keyboardManager.Hook(window);
        }

        internal static void Update() {
            mouseInputManager.Update();
            keyboardManager.Update();
        }

        public static MouseInputManager Mouse {
            get {
                return mouseInputManager;
            }
        }

        public static KeyboardInputManager Keyboard {
            get {
                return keyboardManager;
            }
        }
    }
}
