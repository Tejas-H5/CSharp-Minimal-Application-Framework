using RenderingEngine.Datatypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.UI.Core
{
    public class UIRectTransform
    {
        Rect2D _rect;

        Rect2D _absoluteOffset;
        Rect2D _normalizedAnchoring;
        PointF _normalizedCenter;

        public bool PositionSizeX { get; set; }
        public bool PositionSizeY { get; set; }

        public PointF NormalizedCenter {
            get { return _normalizedCenter; }
            set { _normalizedCenter = value; }
        }

        public Rect2D Rect {
            get { return _rect; }
            set { 
                _rect = value;
            }
        }

        public void SetRectAndRecalcAnchoring(Rect2D value, Rect2D parentRect)
        {
            _rect = value;
            _rect.Rectify();
            ///*
            if (PositionSizeX)
            {
                float anchor = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;
                float absCenter = _rect.X0 + _rect.Width * _normalizedCenter.X - parentRect.X0;
                float xPosition = absCenter - anchor;

                SetAbsPositionSizeX(xPosition, _rect.Width);
            }
            else
            {
                float anchorLeft = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;
                float anchorRight = parentRect.Left + _normalizedAnchoring.X1 * parentRect.Width;

                float leftOffset = _rect.X0 - anchorLeft;
                float rightOffset =  anchorRight - _rect.X1;

                SetAbsOffsetsX(leftOffset, rightOffset);
            }

            if (PositionSizeY)
            {
                float anchor = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;
                float absCenter = _rect.Y0 + _rect.Height * _normalizedCenter.Y - parentRect.Y0;
                float yPosition = absCenter - anchor;

                SetAbsPositionSizeY(yPosition, _rect.Height);
            }
            else
            {
                float anchorBottom = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;
                float anchorTop = parentRect.Bottom + _normalizedAnchoring.Y1 * parentRect.Height;

                float bottomOffset = _rect.Y0 - anchorBottom;
                float topOffset = anchorTop - _rect.Y1;

                SetAbsOffsetsY(bottomOffset, topOffset);
            }
            //*/
        }

        public Rect2D NormalizedAnchoring { get { return _normalizedAnchoring; } set { _normalizedAnchoring = value; } }
        public Rect2D AbsoluteOffset { get { return _absoluteOffset; } set { _absoluteOffset = value; } }


        public void SetAbsOffsetsX(float left, float right)
        {
            PositionSizeX = false;
            _absoluteOffset.X0 = left;
            _absoluteOffset.X1 = right;
        }

        public void SetAbsOffsetsY(float bottom, float top)
        {
            PositionSizeY = false;
            _absoluteOffset.Y0 = bottom;
            _absoluteOffset.Y1 = top;
        }

        public void SetAbsoluteOffset(float offset)
        {
            SetAbsoluteOffset(new Rect2D(offset, offset, offset, offset));
        }

        public void SetAbsoluteOffset(Rect2D rectOffset)
        {
            PositionSizeX = false;
            PositionSizeY = false;

            _absoluteOffset = rectOffset;
        }

        public void SetAbsPositionSizeX(float x, float width)
        {
            PositionSizeX = true;
            _absoluteOffset.X0 = x;
            _absoluteOffset.X1 = width;
        }

        public void SetAbsPositionSizeY(float y, float height)
        {
            PositionSizeY = true;
            _absoluteOffset.Y0 = y;
            _absoluteOffset.Y1 = height;
        }

        public void SetAbsPositionSize(float x, float y, float width, float height)
        {
            PositionSizeX = true;
            PositionSizeY = true;
            _absoluteOffset = new Rect2D(x, y, width, height);
        }


        public void SetNormalizedAnchoringX(float left, float right)
        {
            PositionSizeX = false;
            _normalizedAnchoring.X0 = left;
            _normalizedAnchoring.X1 = right;
        }

        public void SetNormalizedAnchoringY(float bottom, float top)
        {
            PositionSizeY = false;
            _normalizedAnchoring.Y0 = bottom;
            _normalizedAnchoring.Y1 = top;
        }

        public void SetNormalizedAnchoring(Rect2D anchor)
        {
            PositionSizeX = false;
            PositionSizeY = false;
            _normalizedAnchoring = anchor;
        }

        public void SetNormalizedPositionCenterX(float x, float centerX = 0.5f)
        {
            PositionSizeX = true;
            _normalizedAnchoring.X0 = x;
            _normalizedCenter.X = centerX;
        }

        public void SetNormalizedPositionCenterY(float y, float centreY = 0.5f)
        {
            PositionSizeY = true;
            _normalizedAnchoring.Y0 = y;
            _normalizedCenter.Y = centreY;
        }

        public void SetNormalizedPositionCenter(float x, float y, float centerX = 0.5f, float centerY = 0.5f)
        {
            PositionSizeX = true;
            PositionSizeY = true;
            _normalizedAnchoring = new Rect2D(x, y, 0,0);
            _normalizedCenter = new PointF(centerX, centerY);
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
            //_rectOffset contains position in X0, Y0 and width, height in X1 Y1

            float anchorLeft = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;

            float X = _absoluteOffset.X0 + anchorLeft;

            float width = _absoluteOffset.X1;

            float left = X - _normalizedCenter.X * width;
            float right = X + (1.0f - _normalizedCenter.X) * width;

            _rect = new Rect2D(left, _rect.X1, right, _rect.Y1);
        }

        private void UpdateRectPositionSizeY(Rect2D parentRect)
        {
            //_anchoring contains the (still) normalized position position in X0 Y0 
            //_rectOffset contains position in X0, Y0 and width, heighit in X1 Y1

            float anchorBottom = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;

            float Y = _absoluteOffset.Y0 + anchorBottom;

            float height = _absoluteOffset.Y1;

            float bottom = Y - _normalizedCenter.Y * height;
            float top = Y + (1.0f - _normalizedCenter.Y) * height;

            _rect = new Rect2D(_rect.X0, bottom, _rect.X1, top);
        }

        private void UpdateRectOffsetX(Rect2D parentRect)
        {
            float anchorLeft = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;
            float anchorRight = parentRect.Left + _normalizedAnchoring.X1 * parentRect.Width;

            float left = anchorLeft + _absoluteOffset.X0;
            float right = anchorRight - _absoluteOffset.X1;

            _rect = new Rect2D(left, _rect.Y0, right, _rect.Y1);
        }

        private void UpdateRectOffsetY(Rect2D parentRect)
        {
            float anchorBottom = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;
            float anchorTop = parentRect.Bottom + _normalizedAnchoring.Y1 * parentRect.Height;

            float bottom = anchorBottom + _absoluteOffset.Y0;
            float top = anchorTop - _absoluteOffset.Y1;

            _rect = new Rect2D(_rect.X0, bottom, _rect.X1, top);
        }
    }
}
