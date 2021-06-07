using MinimalAF.Logic;
using MinimalAF.UI.Core;
using MinimalAF.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI.Components.MouseInput
{
    [RequiredComponents(typeof(UIMouseListener))]
    public class UIMouseScroll : UIComponent
    {
        UIMouseListener _mouseListener;

        public float ScrollSpeed { get; set; } = 20;
        public UIElement Target { get; set; } = null;

        bool _vertical;

        public UIMouseScroll(bool vertical)
        {
            _vertical = vertical;
        }

        public override void SetParent(UIElement parent)
        {
            if(_mouseListener!=null)
                _mouseListener.OnMousewheelScroll -= OnMouseScroll;

            base.SetParent(parent);

            _mouseListener = parent.GetComponentOfType<UIMouseListener>();

            _mouseListener.OnMousewheelScroll += OnMouseScroll;
        }

        float _currentAmount = 0;

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
        }

        private void OnMouseScroll(MouseEventArgs e)
        {
            if (Target == null)
                return;

            float amount = Input.MouseWheelNotches * ScrollSpeed;
            _currentAmount -= amount;

            _currentAmount = MathF.Max(MathF.Min(_currentAmount, Target.Rect.Height - _parent.Rect.Height), 0);

            int targetInstanceID = Target.GetHashCode();
            int parentInstanceID = _parent.GetHashCode();

            if (_vertical)
            {
                Target.PosSizeY(_currentAmount, Target.Rect.Height);
            }
            else
            {
                Target.PosSizeX(_currentAmount, Target.Rect.Width);
            }

            e.Handled = true;
        }

        public override UIComponent Copy()
        {
            return new UIMouseScroll(_vertical);
        }
    }
}
