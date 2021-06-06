using MinimalAF.Datatypes.ObserverPattern;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.UI.Components.MouseInput;
using MinimalAF.UI.Components.Visuals;
using MinimalAF.UI.Core;
using MinimalAF.UI.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MinimalAF.UI.Components.DataInput
{
    //TODO: Esc to cancel out of an input
    //might have to use another string rather than the string inside the TextComponent
    [RequiredComponents(typeof(UIMouseListener), typeof(UIHitbox)), 
        RequiredComponentsInChildren(typeof(UIText))]
    public abstract class UITextInput<T> : UIDataInput<T>
    {
        protected UIMouseListener _mouseListner;
        protected UIText _textComponent;
        protected UIHitbox _hitbox;

        T _initValue;

        protected bool _isTyping;
        protected bool _shouldClear;
        protected bool _acceptsNewLine = false;

        public UITextInput(T initValue, bool acceptsNewLines, bool shouldClear)
        {
            _shouldClear = shouldClear;
            _acceptsNewLine = acceptsNewLines;
            _initValue = initValue;
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
            _property.OnDataChanged += OnPropertyChanged;

            OnTextChanged += OnTextChangedEvent;

            OnTextFinalized += OnTextFinalizedSelf;
        }

        /// <summary>
        /// What happens when you accept the input that you entered.
        /// The defocusing will not happen here
        /// </summary>
        protected void OnTextFinalizedSelf()
        {
            T val;
            if(!TryParseText(_textComponent.Text, out val))
            {
                val = _initValue;
            }

            _property.Value = val;
            _textComponent.Text = _property.Value.ToString();
        }

        protected abstract bool TryParseText(string s, out T val);

        protected virtual void OnPropertyChanged(T obj)
        {
            string s = "{null}";
            if (Property.Value != null)
                s = Property.Value.ToString();

            _textComponent.Text = s;
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

        private void OnMouseOver(MouseEventArgs e)
        {
            e.Handled = true;

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
