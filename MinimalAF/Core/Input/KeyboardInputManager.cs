using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text;

namespace MinimalAF {
    public class KeyboardInputManager {
        const string KEYBOARD_CHARS = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        OpenTKWindowWrapper window;

        bool[] prevKeyStates = new bool[(int)KeyCode.LastKey];
        bool[] keyStates = new bool[(int)KeyCode.LastKey];


        StringBuilder charactersTypedSB = new StringBuilder();
        string charactersTyped = "";

        bool anyKeyHeld = false;
        bool anyKeyPressed = false;
        bool anyKeyReleased = false;

        internal KeyboardInputManager() {
        }

        private void OnWindowTextInput(uint c) {
            charactersTypedSB.Append((char)c);
        }

        internal void Hook(OpenTKWindowWrapper window) {
            this.window = window;
            window.TextInputEvent += OnWindowTextInput;
        }

        internal void Unhook() {
            if (window != null)
                window.TextInputEvent -= OnWindowTextInput;
        }

        internal bool IsPressed(KeyCode key) {
            if (key == KeyCode.Control) {
                return IsPressed(KeyCode.LeftControl) || IsPressed(KeyCode.RightControl);
            }
            if (key == KeyCode.Shift) {
                return IsPressed(KeyCode.LeftShift) || IsPressed(KeyCode.RightShift);
            }
            if (key == KeyCode.Alt) {
                return IsPressed(KeyCode.LeftAlt) || IsPressed(KeyCode.RightAlt);
            }
            if (key == KeyCode.Any) {
                return anyKeyPressed;
            }
            return (!WasHeld(key)) && (IsHeld(key));
        }

        private bool WasHeld(KeyCode key) {
            return prevKeyStates[(int)key];
        }

        internal bool IsReleased(KeyCode key) {
            if (key == KeyCode.Control) {
                return IsReleased(KeyCode.LeftControl) || IsReleased(KeyCode.RightControl);
            }
            if (key == KeyCode.Shift) {
                return IsReleased(KeyCode.LeftShift) || IsReleased(KeyCode.RightShift);
            }
            if (key == KeyCode.Alt) {
                return IsReleased(KeyCode.LeftAlt) || IsReleased(KeyCode.RightAlt);
            }
            if (key == KeyCode.Any) {
                return anyKeyReleased;
            }

            return WasHeld(key) && (!IsHeld(key));
        }

        internal bool IsHeld(KeyCode key) {
            if (key == KeyCode.Control) {
                return IsHeld(KeyCode.LeftControl) || IsHeld(KeyCode.RightControl);
            }
            if (key == KeyCode.Shift) {
                return IsHeld(KeyCode.LeftShift) || IsHeld(KeyCode.RightShift);
            }
            if (key == KeyCode.Alt) {
                return IsHeld(KeyCode.LeftAlt) || IsHeld(KeyCode.RightAlt);
            }
            if (key == KeyCode.Any) {
                return anyKeyHeld;
            }

            return keyStates[(int)key];
        }

        internal string CharactersTyped {
            get {
                return charactersTyped;
            }
        }

        internal string CharactersPressed {
            get {
                var pressed = new StringBuilder();
                for (int i = 0; i < keyStates.Length - 1; i++) {
                    if (prevKeyStates[i] || !keyStates[i]) {
                        continue;
                    }

                    pressed.Append(CharKeyMapping.KeyCodeToChar((KeyCode)i));
                }

                return pressed.ToString();
            }
        }

        internal string CharactersReleased {
            get {
                var released = new StringBuilder();
                for (int i = 0; i < keyStates.Length - 1; i++) {
                    if (!prevKeyStates[i] || keyStates[i]) {
                        continue;
                    }

                    released.Append(CharKeyMapping.KeyCodeToChar((KeyCode)i));
                }

                return released.ToString();
            }
        }

        internal string CharactersHeld {
            get {
                var held = new StringBuilder();
                for (int i = 0; i < keyStates.Length - 1; i++) {
                    if (!prevKeyStates[i] || !keyStates[i]) {
                        continue;
                    }

                    held.Append(CharKeyMapping.KeyCodeToChar((KeyCode)i));
                }

                return held.ToString();
            }
        }

        internal void Update() {
            anyKeyHeld = false;
            anyKeyPressed = false;
            anyKeyReleased = false;

            charactersTyped = charactersTypedSB.ToString();
            charactersTypedSB.Clear();

            bool[] temp = prevKeyStates;
            prevKeyStates = keyStates;
            keyStates = temp;

            for (int i = 0; i < keyStates.Length - 1; i++) {
                KeyCode key = (KeyCode)i;

                //This is where we use openTK
                keyStates[i] = window.KeyboardState.IsKeyDown((Keys)key);


                anyKeyHeld = anyKeyHeld || keyStates[i];

                bool pressed = (!prevKeyStates[i] && keyStates[i]);
                bool released = (prevKeyStates[i] && !keyStates[i]);

                anyKeyPressed = anyKeyPressed || pressed;
                anyKeyReleased = anyKeyReleased || released;
            }
        }
    }
}

