using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UITextInput : UIComponent
    {
        private UIMouseListener _mouseListner;
        private UIText _textComponent;
        private UIHitbox _hitbox;

        bool _isTyping;

        public override void SetParent(UIElement parent)
        {
            if(_mouseListner != null)
            {
                _mouseListner.OnMouseOver -= OnMouseOver;
            }

            base.SetParent(parent);

            _mouseListner = _parent.GetComponentOfType<UIMouseListener>();
            _textComponent = _parent.GetComponentInChildrenOfType<UIText>();
            _hitbox = _parent.GetComponentOfType<UIHitbox>();
                
            _mouseListner.OnMouseOver += OnMouseOver;
        }

        private void OnMouseOver()
        {
            if (Input.IsMouseClicked(MouseButton.Left))
            {
                _isTyping = true;
            }
        }

        public event Action OnTextChanged;
        private bool _isInvokingOnTextChanged = false;

        private void FireTextChangedEvent()
        {
            if (_isInvokingOnTextChanged)
                return;

            _isInvokingOnTextChanged = true;
            OnTextChanged?.Invoke();
            _isInvokingOnTextChanged = false;
        }

        public override bool ProcessEvents()
        {
            if (_isTyping && Input.IsMouseClicked(MouseButton.Left))
            {
                if(!_hitbox.PointIsInside(Input.MouseX, Input.MouseY))
                {
                    _isTyping = false;
                }
            }

            if (Input.IsKeyPressed(KeyCode.Escape))
            {
                _isTyping = false;
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
            for(int i = 0; i < Input.CharactersTyped.Length; i++)
            {
                if (Input.CharactersTyped[i] == '\b')
                {
                    if(_textComponent.Text.Length > 0)
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
                FireTextChangedEvent();
            }
        }
    }
}
