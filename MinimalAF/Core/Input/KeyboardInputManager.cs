using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Text;

namespace MinimalAF
{
	public class KeyboardInputManager
    {
        const string KEYBOARD_CHARS = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        OpenTKWindowWrapper _window;

        bool[] _prevKeyStates = new bool[(int)KeyCode.LastKey];
        bool[] _keyStates = new bool[(int)KeyCode.LastKey];


        StringBuilder _charactersTypedSB = new StringBuilder();
        StringBuilder _charactersPressedSB = new StringBuilder();
        StringBuilder _charactersReleasedSB = new StringBuilder();
        StringBuilder _charactersDownSB = new StringBuilder();

        string _charactersPressed = "";
        string _charactersTyped = "";
        string _charactersDown = "";
        string _charactersReleased = "";


        bool _anyKeyDown = false;
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

        public bool IsPressed(KeyCode key)
        {
            return _keyStates[(int)key];
        }

        private bool WasPressed(KeyCode key)
        {
            return _prevKeyStates[(int)key];
        }

        public bool IsReleased(KeyCode key)
        {
            return WasPressed(key) && (!IsPressed(key));
        }

        public bool IsDown(KeyCode key)
        {
            return (!WasPressed(key)) && (IsPressed(key));
        }

        public bool IsShiftPressed {
            get {
                return IsDown(KeyCode.LeftShift) || IsDown(KeyCode.RightShift);
            }
        }

        public bool IsCtrlPressed {
            get {
                return IsDown(KeyCode.LeftControl) || IsDown(KeyCode.RightControl);
            }
        }

        public bool IsAltPressed {
            get {
                return IsDown(KeyCode.LeftAlt) || IsDown(KeyCode.RightAlt);
            }
        }

        public bool IsShiftReleased {
            get {
                return IsReleased(KeyCode.LeftShift) || IsReleased(KeyCode.RightShift);
            }
        }

        public bool IsCtrlReleased {
            get {
                return IsReleased(KeyCode.LeftControl) || IsReleased(KeyCode.RightControl);
            }
        }

        public bool IsAltReleased {
            get {
                return IsReleased(KeyCode.LeftAlt) || IsReleased(KeyCode.RightAlt);
            }
        }


        public bool IsShiftDown {
            get {
                return IsPressed(KeyCode.LeftShift) || IsPressed(KeyCode.RightShift);
            }
        }

        public bool IsCtrlDown {
            get {
                return IsPressed(KeyCode.LeftControl) || IsPressed(KeyCode.RightControl);
            }
        }

        public bool IsAltDown {
            get {
                return IsPressed(KeyCode.LeftAlt) || IsPressed(KeyCode.RightAlt);
            }
        }


        public string CharactersPressed { get { return _charactersPressed; } }
        public string CharactersTyped { get { return _charactersTyped; } }
        public string CharactersReleased { get { return _charactersReleased; } }
        public string CharactersDown { get { return _charactersDown; } }


        public bool IsAnyDown { get { return _anyKeyDown; } }
        public bool IsAnyPressed { get { return _anyKeyPressed; } }
        public bool IsAnyReleased { get { return _anyKeyReleased; } }


        public void Update()
        {
            _anyKeyDown = false;
            _anyKeyPressed = false;
            _anyKeyReleased = false;
            _charactersPressedSB.Clear();
            _charactersDownSB.Clear();
            _charactersReleasedSB.Clear();

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


                _anyKeyDown = _anyKeyDown || _keyStates[i];

                bool pressed = (!_prevKeyStates[i] && _keyStates[i]);
                bool released = (_prevKeyStates[i] && !_keyStates[i]);

                char c = CharKeyMapping.KeyCodeToChar(key);
                if (_keyStates[i])
                {
                    _charactersDownSB.Append(c);
                }

                if (pressed)
                {
                    _charactersPressedSB.Append(c);
                }

                if (released)
                {
                    _charactersReleasedSB.Append(c);
                }


                _anyKeyPressed = _anyKeyPressed || pressed;
                _anyKeyReleased = _anyKeyReleased || released;
            }

            _charactersPressed = _charactersPressedSB.ToString();
            _charactersReleased = _charactersReleasedSB.ToString();
            _charactersDown = _charactersDownSB.ToString();
        }
    }
}

