using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Logic
{
    class KeyboardInputManager
    {
        WindowInstance _window;
        public KeyboardInputManager(WindowInstance window)
        {
            _window = window;
        }

        public bool IsKeyDown(KeyCode key)
        {
            return _window.KeyboardState.IsKeyDown((Keys)key);
        }

        public bool IsKeyReleased(KeyCode key)
        {
            return _window.KeyboardState.IsKeyReleased((Keys)key);
        }

        public bool IsKeyPressed(KeyCode key)
        {
            return _window.KeyboardState.IsKeyPressed((Keys)key);
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

        StringBuilder _charactersPressedSB = new StringBuilder();
        StringBuilder _charactersDownSB = new StringBuilder();

        string _charactersPressed = "";
        string _charactersDown = "";

        public string CharactersPressed { get { return _charactersPressed;  } }
        public string CharactersDown { get { return _charactersDown; } }


        const string keyboardChars = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        public void Update()
        {
            _charactersPressedSB.Clear();
            _charactersDownSB.Clear();

            for (int i = 0; i < keyboardChars.Length; i++)
            {
                KeyCode key = CharKeyMapping.CharToKey(keyboardChars[i]);
                if (IsKeyPressed(key))
                {
                    _charactersPressedSB.Append(keyboardChars[i]);
                }

                if (IsKeyDown(key))
                {
                    _charactersDownSB.Append(keyboardChars[i]);
                }
            }

            _charactersPressed = _charactersPressed.ToString();
            _charactersDown = _charactersDownSB.ToString();
        }
    }
}

