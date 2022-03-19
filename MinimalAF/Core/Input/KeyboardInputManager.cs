using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text;

namespace MinimalAF
{
	internal class KeyboardInputManager
    {
        const string KEYBOARD_CHARS = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        OpenTKWindowWrapper _window;

        bool[] _prevKeyStates = new bool[(int)KeyCode.LastKey];
        bool[] _keyStates = new bool[(int)KeyCode.LastKey];


        StringBuilder _charactersTypedSB = new StringBuilder();
        string _charactersTyped = "";

        bool _anyKeyHeld = false;
        bool _anyKeyPressed = false;
        bool _anyKeyReleased = false;

        internal KeyboardInputManager() { }

        private void OnWindowTextInput(uint c)
        {
            _charactersTypedSB.Append((char)c);
        }

        internal void Hook(OpenTKWindowWrapper window)
        {
            _window = window;
            _window.TextInputEvent += OnWindowTextInput;
        }

        internal void Unhook()
        {
            if (_window != null)
                _window.TextInputEvent -= OnWindowTextInput;
        }

        internal bool IsPressed(KeyCode key)
        {
            if(key == KeyCode.Control) {
                return IsPressed(KeyCode.LeftControl) || IsPressed(KeyCode.RightControl);
            }
            if(key == KeyCode.Shift) {
                return IsPressed(KeyCode.LeftShift) || IsPressed(KeyCode.RightShift);
            }
            if(key == KeyCode.Alt) {
                return IsPressed(KeyCode.LeftAlt) || IsPressed(KeyCode.RightAlt);
            }
            if(key == KeyCode.Any) {
                return _anyKeyPressed;
            }

            return _keyStates[(int)key];
        }

        private bool WasPressed(KeyCode key)
        {
            return _prevKeyStates[(int)key];
        }

        internal bool IsReleased(KeyCode key)
        {
            if(key == KeyCode.Control) {
                return IsReleased(KeyCode.LeftControl) || IsReleased(KeyCode.RightControl);
            }
            if(key == KeyCode.Shift) {
                return IsReleased(KeyCode.LeftShift) || IsReleased(KeyCode.RightShift);
            }
            if(key == KeyCode.Alt) {
                return IsReleased(KeyCode.LeftAlt) || IsReleased(KeyCode.RightAlt);
            }
            if(key == KeyCode.Any) {
                return _anyKeyReleased;
            }

            return WasPressed(key) && (!IsPressed(key));
        }

        internal bool IsHeld(KeyCode key)
        {
            if(key == KeyCode.Control) {
                return IsHeld(KeyCode.LeftControl) || IsHeld(KeyCode.RightControl);
            }
            if(key == KeyCode.Shift) {
                return IsHeld(KeyCode.LeftShift) || IsHeld(KeyCode.RightShift);
            }
            if(key == KeyCode.Alt) {
                return IsHeld(KeyCode.LeftAlt) || IsHeld(KeyCode.RightAlt);
            }
            if(key == KeyCode.Any) {
                return _anyKeyHeld;
            }

            return (!WasPressed(key)) && (IsPressed(key));
        }

        internal string CharactersTyped { get { return _charactersTyped; } }

        internal string CharactersPressed { 
            get { 
                var pressed = new StringBuilder();
                for (int i = 0; i < _keyStates.Length - 1; i++) {
                    if(_prevKeyStates[i] || !_keyStates[i]){
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
                for (int i = 0; i < _keyStates.Length - 1; i++) {
                    if(!_prevKeyStates[i] || _keyStates[i]){
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
                for (int i = 0; i < _keyStates.Length - 1; i++) {
                    if(!_prevKeyStates[i] || !_keyStates[i]){
                        continue;
                    }

                    held.Append(CharKeyMapping.KeyCodeToChar((KeyCode)i));
                }

                return held.ToString(); 
            } 
        }

        internal void Update()
        {
            _anyKeyHeld = false;
            _anyKeyPressed = false;
            _anyKeyReleased = false;

            _charactersTyped = _charactersTypedSB.ToString();
            _charactersTypedSB.Clear();

            bool[] temp = _prevKeyStates;
            _prevKeyStates = _keyStates;
            _keyStates = temp;

            for (int i = 0; i < _keyStates.Length - 1; i++)
            {
                KeyCode key = (KeyCode)i;

                //This is where we use openTK
                _keyStates[i] = _window.KeyboardState.IsKeyDown((Keys)key);


                _anyKeyHeld = _anyKeyHeld || _keyStates[i];

                bool pressed = (!_prevKeyStates[i] && _keyStates[i]);
                bool released = (_prevKeyStates[i] && !_keyStates[i]);

                _anyKeyPressed = _anyKeyPressed || pressed;
                _anyKeyReleased = _anyKeyReleased || released;
            }
        }
    }
}

