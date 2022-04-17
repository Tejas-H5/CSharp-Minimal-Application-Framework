using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Drawing;

namespace MinimalAF {
    public interface IInput<T> {
        public event Action<T> OnChanged;
        public event Action<T> OnFinalized;
    }

    public class TextInput<T> : Element, IInput<T> {
        TextElement _textObject, placeholderTextObject;

        bool _endsTypingOnNewline = true;
        float blinkPhase = 0, blinkSpeed = 3;
        public event Action<T> OnChanged;
        public event Action<T> OnFinalized;
        public event Action OnDefocused;

        public string Placeholder {
            get => placeholderTextObject.String;
            set => placeholderTextObject.String = value;
        }

        public string String {
            get => _textObject.String;
            set => _textObject.String = value;
        }

        Property<T> _property;
        public Property<T> Property;

        Func<string, T> _parser;

        UIState _uiState;

        public bool HasFocus {
            get {
                if (_uiState == null) {
                    return false;
                }

                return _uiState.CurrentlyFocused == this;
            }
        }

        public void Focus(bool focus = true) {
            if (focus) {
                _uiState.CurrentlyFocused = this;
            } else if (_uiState.CurrentlyFocused == this) {
                _uiState.CurrentlyFocused = null;
                OnDefocused?.Invoke();
            }
        }

        /// <summary>
        /// A text input that sets value to a property.
        /// If the parsing function throws an exception, the property is never set.
        /// 
        /// ToString is being used to convert from the value back to a string.
        /// </summary>
        public TextInput(TextElement child, T defaultValue, Func<string, T> parser, Func<T, T> validator = null, bool endsTypingOnNewline = true) {
            _textObject = child;
            _property = new Property<T>(defaultValue, validator);
            _parser = parser;
            _endsTypingOnNewline = endsTypingOnNewline;


            Color4 placeholderCol = _textObject.TextColor;
            placeholderCol.A *= 0.5f;

            placeholderTextObject = new TextElement(
                "", placeholderCol, _textObject.Font, _textObject.FontSize, _textObject.VerticalAlignment, _textObject.HorizontalAlignment
            );

            SetChildren(
                _textObject,
                placeholderTextObject
            );

            Clipping = true;

            String = defaultValue.ToString();
        }


        public override void OnMount(Window w) {
            _uiState = GetResource<UIState>();
        }

        public override void OnRender() {
            if (HasFocus) {
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


            if (!HasFocus)
                return;

            if (TypeKeystrokes() || Input.Keyboard.IsPressed(KeyCode.Escape)) {
                EndTyping();
            }
        }

        public override void AfterUpdate() {
            if (!HasFocus)
                return;

            if (Input.Mouse.IsAnyPressed && !MouseOverSelf) {
                Focus(false);
            }
        }


        private bool TypeKeystrokes() {
            string typed = Input.Keyboard.CharactersTyped;
            if (typed.Length == 0)
                return false;

            for (int i = 0; i < typed.Length; i++) {
                if (typed[i] == '\b') {
                    if (String.Length > 0) {
                        String = String.Substring(0, String.Length - 1);
                    }
                } else {
                    String += typed[i];
                }

            }

            string s = String;

            if (s.Length > 0 && s[s.Length - 1] == '\n') {
                String = s.Substring(0, s.Length - 1);

                if (_endsTypingOnNewline)
                    return true;
            }

            try {
                OnChanged?.Invoke(_parser(s));
                placeholderTextObject.IsVisible = (s == "");
            } catch (Exception) {

            }

            return false;
        }


        public void EndTyping() {
            if (!HasFocus)
                return;

            Focus(false);
            bool changed = false;

            try {
                _property.Value = _parser(String);
                changed = true;
            } catch (Exception) {
                // validation error. 
            }

            String = _property.Value.ToString();

            if (changed) {
                OnFinalized?.Invoke(_property.Value);
            }
        }
    }
}
