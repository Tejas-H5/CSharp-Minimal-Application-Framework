using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MinimalAF
{
    public class TextInput<T> : Container
    {
        TextElement _textObject;

        bool _isTyping;
        bool _shouldClear;
        bool _endsTypingOnNewline = true;

        public event Action OnTextChanged;
        public event Action OnTextFinalized;

        Property<T> _property;
        Func<string, T> _parser;

        /// <summary>
        /// A text input that sets value to a property.
        /// If the parsing function throws an exception, the property is never set.
        /// </summary>
        public TextInput(TextElement child, Property<T> property, Func<string,T> parser, bool endsTypingOnNewline = true)
        {
            _textObject = child;
            _property = property;
            _parser = parser;
            _endsTypingOnNewline = endsTypingOnNewline;

            this.SetChildren(
                _textObject
            );

            OnTextFinalized += OnTextFinalizedSelf;
        }

        public override void OnRender()
        {
            if (_isTyping)
            {
                RenderCarat();
            }


            base.OnRender();
        }

        private void RenderCarat()
        {
            PointF caratPos = _textObject.GetCaratPos();
            float height = _textObject.GetCharacterHeight();
            CTX.SetDrawColor(_textObject.TextColor);
            CTX.DrawRect(caratPos.X, caratPos.Y, caratPos.X + 2, caratPos.Y + height);
        }

        public override void OnUpdate()
        {
            if(_isTyping)
            {
                if (Input.Mouse.IsPressed(MouseButton.Left))
                {
                    if (!Input.Mouse.IsOver(Rect))
                    {
                        EndTyping();
                    }
                }

                if (Input.Keyboard.IsPressed(KeyCode.Escape))
                {
                    EndTyping();
                }

                TypeKeystrokes();
            }

            base.OnUpdate();
        }

        public override bool ProcessEvents()
        {
            if (_isTyping)
                return true;

            return WaitForMouseClick();
        }

        private bool WaitForMouseClick()
        {
            if (Input.Mouse.IsPressed(MouseButton.Left) && Input.Mouse.IsOver(Rect))
            {
                _isTyping = true;

                if (_shouldClear)
                    _textObject.Text = "";

                return true;
            }

            return false;
        }


        private bool TypeKeystrokes()
        {
            bool changed = false;
            string typed = Input.Keyboard.CharactersTyped;
            for (int i = 0; i < typed.Length; i++)
            {
                if (typed[i] == '\b')
                {
                    if (_textObject.Text.Length > 0)
                    {
                        _textObject.Text = _textObject.Text.Substring(0, _textObject.Text.Length - 1);
                    }
                }
                else
                {
                    _textObject.Text += typed[i];
                }

                changed = true;
            }

            if (changed)
            {
                string s = _textObject.Text;

                if (s.Length > 0 && s[s.Length - 1] == '\n')
                {
                    if (_endsTypingOnNewline)
                        return true;

                    _textObject.Text = s.Substring(0, s.Length - 1);
                    EndTyping();
                }

                OnTextChanged?.Invoke();
            }

            return true;
        }


        protected void EndTyping()
        {
            if (_isTyping)
            {
                _isTyping = false;
                OnTextFinalized?.Invoke();
            }
        }

        /// <summary>
        /// What happens when you accept the input that you entered.
        /// The defocusing will not happen here
        /// </summary>
        protected void OnTextFinalizedSelf()
        {
            try
            {
                _property.Value = _parser(_textObject.Text);
            }
            catch (Exception e)
            {
                
            }

            _textObject.Text = _property.Value.ToString();
        }
    }
}
