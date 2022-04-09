using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Drawing;

namespace MinimalAF {
    public class TextInput<T> : Element {
        TextElement _textObject;

        bool _shouldClear;
        bool _endsTypingOnNewline = true;

        public event Action OnTextChanged;
        public event Action OnTextFinalized;

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

            this.SetChildren(
                _textObject
            );
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
            CTX.SetDrawColor(_textObject.TextColor);
            CTX.Rect.Draw(caratPos.X, caratPos.Y, caratPos.X + 2, caratPos.Y + height);
        }

        public override void OnUpdate() {
            if (MouseButtonPressed(MouseButton.Any) && MouseOverSelf()) {
                _uiState.CurrentlyFocused = this;
            }

            if (_uiState.CurrentlyFocused != this)
                return;


            if (TypeKeystrokes() || Input.Keyboard.IsPressed(KeyCode.Escape)) {
                EndTyping();
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

            OnTextChanged?.Invoke();
			return false;
        }


        protected void EndTyping() {
			if (_uiState.CurrentlyFocused != this)
				return;

			_uiState.CurrentlyFocused = null;

			try {
				_property.Value = _parser(_textObject.String);
			} catch (Exception e) {
				// validation error. 
			}

			_textObject.String = _property.Value.ToString();

			OnTextFinalized?.Invoke();
        }
    }
}
