using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Drawing;

namespace MinimalAF {
    public class TextInput<T> : Element {
        TextElement _textObject, placeholderTextObject;

        bool _endsTypingOnNewline = true;
        float blinkPhase = 0, blinkSpeed = 3;
        public event Action<T> OnTextChanged;
        public event Action<T> OnTextFinalized;

        public string Placeholder {
            get => placeholderTextObject.String;
            set => placeholderTextObject.String = value;
        }

        Property<T> _property;
        Func<string, T> _parser;

        UIState _uiState;

        /// <summary>
        /// A text input that sets value to a property.
        /// If the parsing function throws an exception, the property is never set.
        /// </summary>
        public TextInput(TextElement child, Property<T> property, Func<string, T> parser, bool endsTypingOnNewline = true) {
            _textObject = child;
            _property = property;
            _parser = parser;
            _endsTypingOnNewline = endsTypingOnNewline;


            Color4 placeholderCol = _textObject.TextColor;
            placeholderCol.A *= 0.5f;

            placeholderTextObject = new TextElement(
                "", placeholderCol, _textObject.Font, _textObject.FontSize, _textObject.VerticalAlignment, _textObject.HorizontalAlignment
            );

            this.SetChildren(
                _textObject,
                placeholderTextObject
            );

            Clipping = true;
        }


        public override void OnMount(Window w) {
            _uiState = GetResource<UIState>();
        }

        public override void OnRender() {
            if (_uiState.CurrentlyFocused == this) {
                RenderCarat();
            }
        }

        private void RenderCarat() {
            PointF caratPos = _textObject.GetCaratPos();
            float height = _textObject.GetCharacterHeight();

            Color4 col = _textObject.TextColor;
            col.A *= MathF.Sin(blinkPhase);
            col.A *= col.A;

            SetDrawColor(col);
            Rect(caratPos.X, caratPos.Y, caratPos.X + 2, caratPos.Y + height);
        }

        public override void OnUpdate() {
            blinkPhase += Time.DeltaTime * blinkSpeed;
            if (blinkPhase > MathF.PI) {
                blinkPhase -= MathF.PI;
            }

            if (MouseButtonPressed(MouseButton.Any) && MouseOverSelf) {
                _uiState.CurrentlyFocused = this;
            }


            if (_uiState.CurrentlyFocused != this)
                return;

            if (TypeKeystrokes() || Input.Keyboard.IsPressed(KeyCode.Escape)) {
                EndTyping();
            }

            if (Input.Mouse.IsAnyPressed && !MouseOverSelf) {
                _uiState.CurrentlyFocused = null;
            }
        }


        private bool TypeKeystrokes() {
            string typed = Input.Keyboard.CharactersTyped;
            if (typed.Length == 0)
                return false;

            for (int i = 0; i < typed.Length; i++) {
                if (typed[i] == '\b') {
                    if (_textObject.String.Length > 0) {
                        _textObject.String = _textObject.String.Substring(0, _textObject.String.Length - 1);
                    }
                } else {
                    _textObject.String += typed[i];
                }

            }

            string s = _textObject.String;

            if (s.Length > 0 && s[s.Length - 1] == '\n') {
                _textObject.String = s.Substring(0, s.Length - 1);

                if (_endsTypingOnNewline)
                    return true;
            }

            try {
                OnTextChanged?.Invoke(_parser(s));
                placeholderTextObject.IsVisible = (s == "");
            } catch (Exception) {

            }

            return false;
        }


        protected void EndTyping() {
            if (_uiState.CurrentlyFocused != this)
                return;

            _uiState.CurrentlyFocused = null;
            bool changed = false;

            try {
                _property.Value = _parser(_textObject.String);
                changed = true;
            } catch (Exception) {
                // validation error. 
            }

            _textObject.String = _property.Value.ToString();

            if (changed) {
                OnTextFinalized?.Invoke(_property.Value);
            }
        }
    }
}
