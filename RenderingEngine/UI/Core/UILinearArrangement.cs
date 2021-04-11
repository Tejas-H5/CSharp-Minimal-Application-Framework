using RenderingEngine.Datatypes;
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
                float side = _reverse ? 0 : 1;
                element.SetNormalizedPositionCenterY(side, side);
            }
            else
            {
                float side = _reverse ? 1 : 0;
                element.SetNormalizedPositionCenterX(side, side);
            }
        }

        public override void ResizeChildren()
        {

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].UpdateRect();
            }


            for (int i = 0; i < _children.Count; i++)
            {
                if (_vertical)
                {
                    _children[i].RectTransform.SetNormalizedAnchoring(
                        new Rect2D(
                            0,
                            i / (float)_children.Count,
                            1,
                            (i + 1) / (float)_children.Count
                            )
                        );

                    _children[i].SetAbsoluteOffset(new Rect2D(_padding / 2f, _padding / 2f, _padding / 2f, _padding / 2f));
                }
            }

            base.ResizeChildren();
        }
    }
}
