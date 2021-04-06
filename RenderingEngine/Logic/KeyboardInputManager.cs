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

        public bool IsKeyDown(Keys key)
        {
            return _window.KeyboardState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return _window.KeyboardState.IsKeyReleased(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return _window.KeyboardState.IsKeyPressed(key);
        }

        public bool IsShiftPressed {
            get {
                return IsKeyPressed(Keys.LeftShift) || IsKeyPressed(Keys.RightShift);
            }
        }

        public bool IsCtrlPressed {
            get {
                return IsKeyPressed(Keys.LeftControl) || IsKeyPressed(Keys.RightControl);
            }
        }

        public bool IsAltPressed {
            get {
                return IsKeyPressed(Keys.LeftAlt) || IsKeyPressed(Keys.RightAlt);
            }
        }

        public bool IsShiftReleased {
            get {
                return IsKeyReleased(Keys.LeftShift) || IsKeyReleased(Keys.RightShift);
            }
        }

        public bool IsCtrlReleased {
            get {
                return IsKeyReleased(Keys.LeftControl) || IsKeyReleased(Keys.RightControl);
            }
        }

        public bool IsAltReleased {
            get {
                return IsKeyReleased(Keys.LeftAlt) || IsKeyReleased(Keys.RightAlt);
            }
        }


        public bool IsShiftDown {
            get {
                return IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
            }
        }

        public bool IsCtrlDown {
            get {
                return IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
            }
        }

        public bool IsAltDown {
            get {
                return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
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
                Keys key = CharKeyMapping.CharToKey(keyboardChars[i]);
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

