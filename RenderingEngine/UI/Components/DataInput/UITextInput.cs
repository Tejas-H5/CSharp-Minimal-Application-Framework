using RenderingEngine.Datatypes.ObserverPattern;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    //TODO: Esc to cancel out of an input
    //might have to use another string rather than the string inside the TextComponent
    public abstract class UITextInput : UIComponent
    {
        protected UIMouseListener _mouseListner;
        protected UIText _textComponent;
        protected UIHitbox _hitbox;

        bool _isTyping;
        bool _shouldClear;
        bool _acceptsNewLine = false;

        public string Placeholder { get; set; }


        public UITextInput(string placeholder, bool acceptsNewLines, bool shouldClear)
        {
            _shouldClear = shouldClear;
            _acceptsNewLine = acceptsNewLines;
            Placeholder = placeholder;
        }


        public event Action OnTextChanged {
            add { _onTextChanged.Event += value; }
            remove { _onTextChanged.Event -= value; }
        }

        public event Action OnTextFinalized {
            add { _onTextFinalized.Event += value; }
            remove { _onTextFinalized.Event -= value; }
        }

        NonRecursiveEvent _onTextChanged = new NonRecursiveEvent();
        NonRecursiveEvent _onTextFinalized = new NonRecursiveEvent();

        protected void EndTyping()
        {
            if (_isTyping)
            {
                _isTyping = false;
                _onTextFinalized.Invoke();
            }
        }

        public override void SetParent(UIElement parent)
        {
            if (_mouseListner != null)
            {
                _mouseListner.OnMouseOver -= OnMouseOver;
            }

            base.SetParent(parent);

            _mouseListner = _parent.GetComponentOfType<UIMouseListener>();
            _textComponent = _parent.GetComponentInChildrenOfType<UIText>();
            _hitbox = _parent.GetComponentOfType<UIHitbox>();

            _mouseListner.OnMouseOver += OnMouseOver;

            OnTextChanged += OnTextChangedEvent;
        }

        private void OnTextChangedEvent()
        {
            string s = _textComponent.Text;

            if (s.Length > 0 && s[s.Length - 1] == '\n')
            {
                if (_acceptsNewLine)
                    return;

                _textComponent.Text = s.Substring(0, s.Length - 1);
                EndTyping();
            }
        }

        private void OnMouseOver()
        {
            if (Input.IsMouseClicked(MouseButton.Left))
            {
                _isTyping = true;

                if (_shouldClear)
                    _textComponent.Text = "";
            }
        }

        public override bool ProcessEvents()
        {
            if (_isTyping)
            {
                if (_isTyping && Input.IsMouseClicked(MouseButton.Left))
                {
                    if (!_hitbox.PointIsInside(Input.MouseX, Input.MouseY))
                    {
                        EndTyping();
                        return true;
                    }
                }

                if (Input.IsKeyPressed(KeyCode.Escape))
                {
                    EndTyping();
                    return true;
                }
            }

            if (_isTyping)
            {
                TypeKeystrokes();
            }

            return _isTyping;
        }

        public override void Draw(double deltaTime)
        {
            if (!_isTyping)
            {
                return;
            }

            PointF caratPos = _textComponent.GetCaratPos();
            float height = _textComponent.GetCharacterHeight();
            CTX.SetDrawColor(_textComponent.TextColor);
            CTX.DrawRect(caratPos.X, caratPos.Y, caratPos.X + 2, caratPos.Y + height);
        }

        private void TypeKeystrokes()
        {
            bool changed = false;
            for (int i = 0; i < Input.CharactersTyped.Length; i++)
            {
                if (Input.CharactersTyped[i] == '\b')
                {
                    if (_textComponent.Text.Length > 0)
                    {
                        _textComponent.Text = _textComponent.Text.Substring(0, _textComponent.Text.Length - 1);
                    }
                }
                else
                {
                    _textComponent.Text += Input.CharactersTyped[i];
                }

                changed = true;
            }

            if (changed)
            {
                _onTextChanged.Invoke();
            }
        }
    }
}
