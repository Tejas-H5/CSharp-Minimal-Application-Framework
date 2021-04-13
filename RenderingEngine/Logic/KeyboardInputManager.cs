using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Text;

namespace RenderingEngine.Logic
{
    class KeyboardInputManager
    {
        WindowInstance _window;

        bool[] _prevKeyStates = new bool[(int)KeyCode.LastKey];
        bool[] _keyStates = new bool[(int)KeyCode.LastKey];


        private void OnWindowTextInput(uint c)
        {
            _charactersTypedSB.Append((char)c);
        }

        internal void Hook(WindowInstance window)
        {
            _window = window;
            _window.TextInputEvent += OnWindowTextInput;
        }

        internal void Unhook()
        {
			if(_window != null)
            	_window.TextInputEvent -= OnWindowTextInput;
        }

        public bool IsKeyDown(KeyCode key)
        {
            return _keyStates[(int)key];
        }

        private bool WasKeyDown(KeyCode key)
        {
            return _prevKeyStates[(int)key];
        }

        public bool IsKeyReleased(KeyCode key)
        {
            return WasKeyDown(key) && (!IsKeyDown(key));
        }

        public bool IsKeyPressed(KeyCode key)
        {
            return (!WasKeyDown(key)) && (IsKeyDown(key));
        }

        public bool IsShiftPressed {
            get {
                return IsKeyPressed(KeyCode.LeftShift) || IsKeyPressed(KeyCode.RightShift);
            }
        }

        public bool IsCtrlPressed {
            get {
                return IsKeyPressed(KeyCode.LeftControl) || IsKeyPressed(KeyCode.RightControl);
            }
        }

        public bool IsAltPressed {
            get {
                return IsKeyPressed(KeyCode.LeftAlt) || IsKeyPressed(KeyCode.RightAlt);
            }
        }

        public bool IsShiftReleased {
            get {
                return IsKeyReleased(KeyCode.LeftShift) || IsKeyReleased(KeyCode.RightShift);
            }
        }

        public bool IsCtrlReleased {
            get {
                return IsKeyReleased(KeyCode.LeftControl) || IsKeyReleased(KeyCode.RightControl);
            }
        }

        public bool IsAltReleased {
            get {
                return IsKeyReleased(KeyCode.LeftAlt) || IsKeyReleased(KeyCode.RightAlt);
            }
        }


        public bool IsShiftDown {
            get {
                return IsKeyDown(KeyCode.LeftShift) || IsKeyDown(KeyCode.RightShift);
            }
        }

        public bool IsCtrlDown {
            get {
                return IsKeyDown(KeyCode.LeftControl) || IsKeyDown(KeyCode.RightControl);
            }
        }

        public bool IsAltDown {
            get {
                return IsKeyDown(KeyCode.LeftAlt) || IsKeyDown(KeyCode.RightAlt);
            }
        }

        StringBuilder _charactersTypedSB = new StringBuilder();

        StringBuilder _charactersPressedSB = new StringBuilder();
        StringBuilder _charactersReleasedSB = new StringBuilder();
        StringBuilder _charactersDownSB = new StringBuilder();

        string _charactersPressed = "";
        string _charactersTyped = "";
        string _charactersDown = "";
        string _charactersReleased = "";

        public string CharactersPressed { get { return _charactersPressed; } }
		public string CharactersTyped { get { return _charactersTyped; } }
        public string CharactersReleased { get { return _charactersReleased; } }
        public string CharactersDown { get { return _charactersDown; } }


        const string keyboardChars = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        bool _anyKeyDown = false;
        bool _anyKeyPressed = false;
        bool _anyKeyReleased = false;

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

