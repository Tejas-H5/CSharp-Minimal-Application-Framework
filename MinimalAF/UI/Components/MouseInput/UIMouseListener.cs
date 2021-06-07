using MinimalAF.Logic;
using System;

namespace MinimalAF.UI
{
    [RequiredComponents(typeof(UIHitbox))]
    public class UIMouseListener : UIComponent
    {
        public event Action<MouseEventArgs> OnMouseOver;
        public event Action<MouseEventArgs> OnMouseEnter;
        public event Action<MouseEventArgs> OnMouseLeave;
        public event Action<MouseEventArgs> OnMousePressed;
        public event Action<MouseEventArgs> OnMouseReleased;
        public event Action<MouseEventArgs> OnMouseHeld;
        public event Action<MouseEventArgs> OnMousewheelScroll;

        public bool IsMouseOver { get { return _isMouseOver; } }
        public bool WasMouseOver { get { return _wasMouseOver; } }

        public bool IsProcessingEvents { get { return _isProcessingEvents; } }

        bool _wasMouseOver = false;
        bool _isMouseOver;

        private UIHitbox _hitbox;

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);
            _hitbox = _parent.GetComponentOfType<UIHitbox>();
        }

        public override void Update(double deltaTime)
        {
            if (_wasProcessingEvents && !_isProcessingEvents)
            {
                _isMouseOver = false;
                _wasMouseOver = false;
                OnMouseLeave?.Invoke(null);
            }

            _wasProcessingEvents = _isProcessingEvents;
            _isProcessingEvents = false;
        }

        bool _wasProcessingEvents = false;
        bool _isProcessingEvents;

        public override void Draw(double deltaTime)
        {
            /*
            if (_isProcessingEvents)
            {
                CTX.SetDrawColor(1, 0, 0, 1);
                CTX.DrawRect(_parent.Rect.Left + 5, _parent.Rect.Top - 5, _parent.Rect.Left + 15, _parent.Rect.Top - 15);
            }
            */
        }

        public void ProcessMouseWheelEvents(MouseEventArgs e)
        {
            if (Input.MouseWheelNotches != 0)
            {
                OnMousewheelScroll?.Invoke(e);
            }
        }

        public void ProcessMouseButtonEvents(MouseEventArgs e)
        {
            _isProcessingEvents = true;

            _wasMouseOver = _isMouseOver;
            _isMouseOver = false;
            _isMouseOver = true;

            if (_wasMouseOver)
            {
                OnMouseOver?.Invoke(e);
            }
            else
            {
                OnMouseEnter?.Invoke(e);
            }

            if (Input.IsMouseClickedAny)
            {
                OnMousePressed?.Invoke(e);
            }

            if (Input.IsMouseDownAny)
            {
                OnMouseHeld?.Invoke(e);
            }

            if (Input.IsMouseReleasedAny)
            {
                OnMouseReleased?.Invoke(e);
            }
        }

        public override UIComponent Copy()
        {
            return new UIMouseListener();
        }
    }
}
