using RenderingEngine.Datatypes;
using RenderingEngine.Util;
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

        public float Width {
            get {
                return _rect.Width;
            }
        }
        public float Height {
            get {
                return _rect.Height;
            }
        }


        public Rect2D NormalizedAnchoring { 
            get { return _normalizedAnchoring; } 
            set { 
                _normalizedAnchoring = value;
                _normalizedAnchoring.X0 = MathUtil.Clamp01(_normalizedAnchoring.X0);
                _normalizedAnchoring.X1 = MathUtil.Clamp01(_normalizedAnchoring.X1);
                _normalizedAnchoring.Y0 = MathUtil.Clamp01(_normalizedAnchoring.Y0);
                _normalizedAnchoring.Y1 = MathUtil.Clamp01(_normalizedAnchoring.Y1);

                _normalizedAnchoring.X1 = MathF.Max(_normalizedAnchoring.X0, _normalizedAnchoring.X1);
                _normalizedAnchoring.Y1 = MathF.Max(_normalizedAnchoring.Y0, _normalizedAnchoring.Y1);
            } 
        }

        public Rect2D AbsoluteOffset { get { return _absoluteOffset; } set { _absoluteOffset = value; } }

        public void SetAbsOffsetsX(float left, float right)
        {
            _absoluteOffset.X0 = left;
            _absoluteOffset.X1 = right;
        }

        public void SetAbsOffsetsY(float bottom, float top)
        {
            _absoluteOffset.Y0 = bottom;
            _absoluteOffset.Y1 = top;
        }

        public void SetAbsoluteOffset(float offset)
        {
            SetAbsoluteOffset(new Rect2D(offset, offset, offset, offset));
        }

        public void SetAbsoluteOffset(Rect2D rectOffset)
        {
            _absoluteOffset = rectOffset;
        }

        public void SetAbsPositionSizeX(float x, float width)
        {
            _absoluteOffset.X0 = x - _normalizedCenter.X * width;
            _absoluteOffset.X1 = -x - ((1.0f - _normalizedCenter.X) * width);
        }

        public void SetAbsPositionSizeY(float y, float height)
        {
            _absoluteOffset.Y0 = y - _normalizedCenter.Y * height;
            _absoluteOffset.Y1 = -y - ((1.0f - _normalizedCenter.Y) * height);
        }

        public void SetAbsPositionSize(float x, float y, float width, float height)
        {
            SetAbsPositionSizeX(x, width);
            SetAbsPositionSizeY(y, height);
        }


        public void SetNormalizedAnchoringX(float left, float right)
        {
            _normalizedAnchoring.X0 = left;
            _normalizedAnchoring.X1 = right;

            NormalizedAnchoring = _normalizedAnchoring;
        }

        public void SetNormalizedAnchoringY(float bottom, float top)
        {
            _normalizedAnchoring.Y0 = bottom;
            _normalizedAnchoring.Y1 = top;

            NormalizedAnchoring = _normalizedAnchoring;
        }

        public void SetNormalizedAnchoring(Rect2D anchor)
        {
            NormalizedAnchoring = anchor;
        }

        public void SetNormalizedPositionCenterX(float x, float centerX = 0.5f)
        {
            _normalizedAnchoring.X0 = x;
            _normalizedAnchoring.X1 = x;

            _normalizedCenter.X = centerX;
            
            NormalizedAnchoring = _normalizedAnchoring;
        }

        public void SetNormalizedPositionCenterY(float y, float centreY = 0.5f)
        {
            _normalizedAnchoring.Y0 = y;
            _normalizedAnchoring.Y1 = y;

            _normalizedCenter.Y = centreY;

            NormalizedAnchoring = _normalizedAnchoring;
        }

        public void SetNormalizedPositionCenter(float x, float y, float centerX = 0.5f, float centerY = 0.5f)
        {
            NormalizedAnchoring = new Rect2D(x, y, x, y);
            _normalizedCenter = new PointF(centerX, centerY);
        }

        public void UpdateRect(Rect2D parentRect)
        {
            float anchorLeft = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;
            float anchorRight = parentRect.Left + _normalizedAnchoring.X1 * parentRect.Width;
            float anchorBottom = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;
            float anchorTop = parentRect.Bottom + _normalizedAnchoring.Y1 * parentRect.Height;

            float left = anchorLeft + _absoluteOffset.X0;
            float right = anchorRight - _absoluteOffset.X1;
            float bottom = anchorBottom + _absoluteOffset.Y0;
            float top = anchorTop - _absoluteOffset.Y1;

            _rect = new Rect2D(left, bottom, right, top);
        }
    }
}
