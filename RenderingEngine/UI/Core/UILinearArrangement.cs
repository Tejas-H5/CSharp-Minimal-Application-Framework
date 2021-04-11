using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Core
{
    class UILinearArrangement : UIElement
    {
        private bool _vertical;
        private bool _reverse;
        private float _padding;

        public UILinearArrangement(bool vertical, bool reverse, float padding)
        {
            _vertical = vertical;
            _reverse = reverse;
            _padding = padding;
        }

        protected override void AddChildVirtual(UIElement element)
        {
            base.AddChildVirtual(element);
            element.RectTransform.PositionSizeX = true;
            element.RectTransform.PositionSizeY = true;

            if (_vertical)
            {
                element.SetNormalizedCenter(0.5f, 1f);
            }
            else
            {
                element.SetNormalizedCenter(0f, 0.5f);
            }

            if (_reverse)
            {
                element.SetNormalizedCenter(
                    (1.0f - element.RectTransform.NormalizedCenter.X),
                    (1.0f - element.RectTransform.NormalizedCenter.Y)
                );
            }
        }

        public override void ResizeChildren()
        {
            base.ResizeChildren();

            float amountX = 0, amountY = 0;

            if (_vertical)
            {
                amountY = 1;
            }
            else
            {
                amountX = 1;
            }

            if (_reverse)
            {
                amountX = -amountX;
                amountY = -amountY;
            }

            for(int i = 0; i < _children.Count; i++)
            {

            }
        }
    }
}
