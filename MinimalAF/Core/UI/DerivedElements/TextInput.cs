using OpenTK.Mathematics;
using System;


namespace MinimalAF {
    public interface IInput<T> {
        public event Action<T> OnChanged;
        public event Action<T> OnFinalized;

        public T Value {
            get; set;
        }

        public bool HasFocus {
            get;
        }
    }

    public class TextInput<T> : Element, IInput<T> {
        TextElement textObject, placeholderTextObject;

        bool endsTypingOnNewline = true;
        float blinkPhase = 0, blinkSpeed = 3;

        public event Action<T> OnFinalized {
            add {
                property.OnChanged += value;
            }
            remove {
                property.OnChanged -= value;
            }
        }

        public event Action<T> OnChanged;
        public event Action OnDefocused;

        public string Placeholder {
            get => placeholderTextObject.String;
            set => placeholderTextObject.String = value;
        }

        public string String {
            get => textObject.String;
            set => textObject.String = value;
        }

        Property<T> property;

        public T Value {
            get => property.Value;
            set => property.Value = value;
        }

        Func<string, T> parser;

        UIState uiState;

        public bool HasFocus {
            get {
                if (uiState == null) {
                    return false;
                }

                return uiState.CurrentlyFocused == this;
            }
        }

        public void Focus(bool focus = true) {
            if (focus) {
                uiState.CurrentlyFocused = this;
            } else if (uiState.CurrentlyFocused == this) {
                uiState.CurrentlyFocused = null;
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
            textObject = child;
            property = new Property<T>(defaultValue, validator);
            this.parser = parser;
            this.endsTypingOnNewline = endsTypingOnNewline;


            Color placeholderCol = textObject.TextColor;
            placeholderCol.A *= 0.5f;

            placeholderTextObject = new TextElement(
                "", placeholderCol, textObject.Font, textObject.FontSize, textObject.VerticalAlignment, textObject.HorizontalAlignment
            );

            SetChildren(
                textObject,
                placeholderTextObject
            );

            Clipping = true;

            String = defaultValue.ToString();

            OnFinalized += TextInput_OnFinalized;
        }

        private void TextInput_OnFinalized(T obj) {
            String = obj.ToString();
        }

        public override void OnMount(Window w) {
            uiState = GetResource<UIState>();
        }

        public override void OnRender() {
            if (HasFocus) {
                RenderCarat();
            }
        }

        private void RenderCarat() {
            Vector2 caratPos = textObject.GetCaratPos();
            float height = textObject.GetCharacterHeight();

            Color col = textObject.TextColor;
            col.A *= MathF.Sin(blinkPhase);
            col.A *= col.A;

            SetDrawColor(col);
            DrawRect(caratPos.X, caratPos.Y, caratPos.X + 2, caratPos.Y + height);
        }

        public override void OnUpdate() {
            blinkPhase += Time.DeltaTime * blinkSpeed;
            if (blinkPhase > MathF.PI) {
                blinkPhase -= MathF.PI;
            }

            if (MouseButtonPressed(MouseButton.Any) && MouseOverSelf) {
                uiState.CurrentlyFocused = this;
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

        public override Rect DefaultRect(float parentWidth, float parentHeight) {
            return textObject.DefaultRect(parentWidth, parentHeight);
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

                if (endsTypingOnNewline)
                    return true;
            }

            try {
                OnChanged?.Invoke(parser(s));
                placeholderTextObject.IsVisible = (s == "");
            } catch (Exception) {

            }

            return false;
        }


        public void EndTyping() {
            if (!HasFocus)
                return;

            Focus(false);

            try {
                property.Value = parser(String);
            } catch (Exception) {
                // validation error. 
                // TODO: do something with this?
            }
        }
    }
}
