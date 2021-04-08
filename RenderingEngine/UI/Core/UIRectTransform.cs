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

        public bool PositionSizeX { get; set; }
        public bool PositionSizeY { get; set; }

        public Rect2D Rect {
            get { return _rect; }
        }

        public Rect2D Anchoring { get { return _anchoring; } set { _anchoring = value; } }
        public Rect2D RectOffset { get { return _rectOffset; } set { _rectOffset = value; } }


        public void SetRectOffsetsX(float left, float right)
        {
            PositionSizeX = false;
            _rectOffset.X0 = left;
            _rectOffset.X1 = right;
        }

        public void SetRectOffsetsY(float bottom, float top)
        {
            PositionSizeY = false;
            _rectOffset.Y0 = bottom;
            _rectOffset.Y1 = top;
        }

        public void SetRectOffset(float offset)
        {
            SetRectOffset(new Rect2D(offset, offset, offset, offset));
        }

        public void SetRectOffset(Rect2D rectOffset)
        {
            PositionSizeX = false;
            PositionSizeY = false;

            _rectOffset = rectOffset;
        }

        public void SetPositionSizeX(float x, float width)
        {
            PositionSizeX = true;
            _rectOffset.X0 = x;
            _rectOffset.X1 = width;
        }

        public void SetPositionSizeY(float y, float height)
        {
            PositionSizeY = true;
            _rectOffset.Y0 = y;
            _rectOffset.Y1 = height;
        }

        public void SetRectPositionSize(float x, float y, float width, float height)
        {
            PositionSizeX = true;
            PositionSizeY = true;
            _rectOffset = new Rect2D(x, y, width, height);
        }


        public void SetAnchoringOffsetsX(float left, float right)
        {
            PositionSizeX = false;
            _anchoring.X0 = left;
            _anchoring.X1 = right;
        }

        public void SetAnchoringOffsetsY(float bottom, float top)
        {
            PositionSizeY = false;
            _anchoring.Y0 = bottom;
            _anchoring.Y1 = top;
        }

        public void SetAnchoringOffset(Rect2D anchor)
        {
            PositionSizeX = false;
            PositionSizeY = false;
            _anchoring = anchor;
        }

        public void SetAnchoringPositionCenterX(float x, float centreX = 0.5f)
        {
            PositionSizeX = true;
            _anchoring.X0 = x;
            _anchoring.X1 = centreX;
        }

        public void SetAnchoringPositionCenterY(float y, float centreY = 0.5f)
        {
            PositionSizeY = true;
            _anchoring.Y0 = y;
            _anchoring.Y1 = centreY;
        }

        public void SetAnchoringPositionCenter(float x, float y, float centreX = 0.5f, float centreY = 0.5f)
        {
            PositionSizeX = true;
            PositionSizeY = true;
            _anchoring = new Rect2D(x, y, centreX, centreY);
        }


        public void UpdateRect(Rect2D parentRect)
        {
            if (PositionSizeX)
            {
                UpdateRectPositionSizeX(parentRect);
            }
            else
            {
                UpdateRectOffsetX(parentRect);
            }

            if (PositionSizeY)
            {
                UpdateRectPositionSizeY(parentRect);
            }
            else
            {
                UpdateRectOffsetY(parentRect);
            }
        }

        private void UpdateRectPositionSizeX(Rect2D parentRect)
        {
            //_anchoring contains the (still) normalized position position in X0 Y0 and normalized center in X1,Y1
            //_rectOffset contains position in X0, Y0 and width, heighit in X1 Y1

            float anchorLeft = parentRect.Left + _anchoring.X0 * parentRect.Width;

            float X = _rectOffset.X0 + anchorLeft;

            float width = _rectOffset.X1;

            float left = X - _anchoring.X1 * width;
            float right = X + (1.0f - _anchoring.X1) * width;

            _rect = new Rect2D(left, _rect.X1, right, _rect.Y1);
        }

        private void UpdateRectPositionSizeY(Rect2D parentRect)
        {
            //_anchoring contains the (still) normalized position position in X0 Y0 and normalized center in X1,Y1
            //_rectOffset contains position in X0, Y0 and width, heighit in X1 Y1

            float anchorBottom = parentRect.Bottom + _anchoring.Y0 * parentRect.Height;

            float Y = _rectOffset.Y0 + anchorBottom;

            float height = _rectOffset.Y1;

            float bottom = Y - _anchoring.Y1 * height;
            float top = Y + (1.0f - _anchoring.Y1) * height;

            _rect = new Rect2D(_rect.X0, bottom, _rect.X1, top);
        }

        private void UpdateRectOffsetX(Rect2D parentRect)
        {
            float anchorLeft = parentRect.Left + _anchoring.X0 * parentRect.Width;
            float anchorRight = parentRect.Left + _anchoring.X1 * parentRect.Width;

            float left = anchorLeft + _rectOffset.X0;
            float right = anchorRight - _rectOffset.X1;

            _rect = new Rect2D(left, _rect.Y0, right, _rect.Y1);
        }

        private void UpdateRectOffsetY(Rect2D parentRect)
        {
            float anchorBottom = parentRect.Bottom + _anchoring.Y0 * parentRect.Height;
            float anchorTop = parentRect.Bottom + _anchoring.Y1 * parentRect.Height;

            float bottom = anchorBottom + _rectOffset.Y0;
            float top = anchorTop - _rectOffset.Y1;

            _rect = new Rect2D(_rect.X0, bottom, _rect.X1, top);
        }
    }
}
