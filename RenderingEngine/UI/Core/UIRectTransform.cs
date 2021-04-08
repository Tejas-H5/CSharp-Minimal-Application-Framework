using RenderingEngine.Datatypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Core
{
    public class UIRectTransform
    {
        Rect2D _rect;

        Rect2D _rectOffset;
        Rect2D _anchoring;

        public bool PositionSize { get; set; }

        public Rect2D Rect {
            get { return _rect; }
        }

        public Rect2D Anchoring { get { return _anchoring; } set { _anchoring = value; } }
        public Rect2D RectOffset { get { return _rectOffset; } set { _rectOffset = value; } }

        public void UpdateRect(Rect2D parentRect)
        {
            if (PositionSize)
            {
                UpdateRectPositionSize(parentRect);
            }
            else
            {
                UpdateRectOffset(parentRect);
            }
        }

        private void UpdateRectPositionSize(Rect2D parentRect)
        {
            //_anchoring contains the (still) normalized position position in X0 Y0 and normalized center in X1,Y1
            //_rectOffset contains position in X0, Y0 and width, heighit in X1 Y1

            float anchorLeft = parentRect.Left + _anchoring.X0 * parentRect.Width;
            float anchorBottom = parentRect.Bottom + _anchoring.Y0 * parentRect.Height;

            float X = _rectOffset.X0 + anchorLeft;
            float Y = _rectOffset.Y0 + anchorBottom;

            float width = _rectOffset.X1;
            float height = _rectOffset.Y1;

            float left = X - _anchoring.X1 * width;
            float bottom = Y - _anchoring.Y1 * height;
            float right = X + (1.0f - _anchoring.X1) * width;
            float top = Y + (1.0f - _anchoring.Y1) * height;

            _rect = new Rect2D(left, bottom, right, top);
        }

        private void UpdateRectOffset(Rect2D parentRect)
        {
            float anchorLeft = parentRect.Left + _anchoring.X0 * parentRect.Width;
            float anchorBottom = parentRect.Bottom + _anchoring.Y0 * parentRect.Height;
            float anchorRight = parentRect.Left + _anchoring.X1 * parentRect.Width;
            float anchorTop = parentRect.Bottom + _anchoring.Y1 * parentRect.Height;

            float left = anchorLeft + _rectOffset.X0;
            float bottom = anchorBottom + _rectOffset.Y0;
            float right = anchorRight - _rectOffset.X1;
            float top = anchorTop - _rectOffset.Y1;

            _rect = new Rect2D(left, bottom, right, top);
        }
    }
}
