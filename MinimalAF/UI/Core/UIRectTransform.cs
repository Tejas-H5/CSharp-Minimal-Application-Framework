using MinimalAF.Datatypes;
using MinimalAF.Util;
using System;
using System.Drawing;

namespace MinimalAF.UI.Core
{
    public class UIRectTransform : ObservableData<UIRectTransform>
    {
        Rect2D _rect;
        Rect2D _absoluteOffset;
        Rect2D _normalizedAnchoring;
        PointF _normalizedCenter;

        public UIRectTransform()
        {
        }

        public Rect2D Rect {
            get { return _rect; }
            set {
                _rect = value;
                DataChanged(this);
            }
        }

        public PointF NormalizedCenter {
            get { return _normalizedCenter; }
            set { 
                _normalizedCenter = value;
                DataChanged(this);
            }
        }

        public Rect2D AbsoluteOffset {
            get { return _absoluteOffset; }
            set {
                _absoluteOffset = value;
                DataChanged(this);
            }
        }

        public PointF AnchoredPositionAbs {
            get {
                return new PointF(
                    _absoluteOffset.X0 + _normalizedCenter.X * _rect.Width,
                    _absoluteOffset.Y0 + _normalizedCenter.Y * _rect.Height
                );
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
                DataChanged(this);
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

        //Not used in the updating process
        public bool PositionSizeX {
            get {
                return MathF.Abs(_normalizedAnchoring.X0 - _normalizedAnchoring.X1) < 0.00001f;
            }
        }

        //Not used in the updating process
        public bool PositionSizeY {
            get {
                return MathF.Abs(_normalizedAnchoring.Y0 - _normalizedAnchoring.Y1) < 0.00001f;
            }
        }

        public void SetAbsOffsetsX(float left, float right)
        {
            _absoluteOffset.X0 = left;
            _absoluteOffset.X1 = right;
            DataChanged(this);
        }

        public void SetAbsOffsetsY(float bottom, float top)
        {
            _absoluteOffset.Y0 = bottom;
            _absoluteOffset.Y1 = top;
            DataChanged(this);
        }

        public void SetAbsoluteOffset(float offset)
        {
            SetAbsoluteOffset(new Rect2D(offset, offset, offset, offset));
        }

        public void SetAbsoluteOffset(Rect2D rectOffset)
        {
            AbsoluteOffset = rectOffset;
        }

        public void SetAbsPositionSizeX(float x, float width)
        {
            _absoluteOffset.X0 = x - _normalizedCenter.X * width;
            _absoluteOffset.X1 = -x - ((1.0f - _normalizedCenter.X) * width);
            DataChanged(this);
        }

        public void SetAbsPositionSizeY(float y, float height)
        {
            _absoluteOffset.Y0 = y - _normalizedCenter.Y * height;
            _absoluteOffset.Y1 = -y - ((1.0f - _normalizedCenter.Y) * height);
            DataChanged(this);
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

        public void SetNormalizedPosition(float x, float y)
        {
            NormalizedAnchoring = new Rect2D(x, y, x, y);
        }

        public void SetNormalizedCenter(float x = 0.5f, float y = 0.5f)
        {
            NormalizedCenter = new PointF(x, y);
        }

        public void SetNormalizedPositionX(float x)
        {
            NormalizedAnchoring = new Rect2D(x, _normalizedAnchoring.Y0, x, _normalizedAnchoring.Y1);
        }

        public void SetNormalizedPositionY(float y)
        {
            NormalizedAnchoring = new Rect2D(_normalizedAnchoring.X0, y, _normalizedAnchoring.X1, y);
        }

        public void SetNormalizedCenterX(float x)
        {
            NormalizedCenter = new PointF(x, _normalizedCenter.Y);
        }

        public void SetNormalizedCenterY(float y)
        {
            NormalizedCenter = new PointF(_normalizedCenter.X, y);
        }

        public void UpdateRectFromOffset(Rect2D parentRect)
        {
            float anchorLeft, anchorRight, anchorBottom, anchorTop;
            GetAnchors(parentRect, out anchorLeft, out anchorRight, out anchorBottom, out anchorTop);

            float left = anchorLeft + _absoluteOffset.X0;
            float right = anchorRight - _absoluteOffset.X1;
            float bottom = anchorBottom + _absoluteOffset.Y0;
            float top = anchorTop - _absoluteOffset.Y1;

            _rect = new Rect2D(left, bottom, right, top);
        }

        public void UpdateOffsetFromRect(Rect2D parentRect)
        {
            float anchorLeft, anchorRight, anchorBottom, anchorTop;
            GetAnchors(parentRect, out anchorLeft, out anchorRight, out anchorBottom, out anchorTop);

            float left = _rect.X0 - anchorLeft;
            float right =  anchorRight - _rect.X1;
            float bottom = _rect.Y0 - anchorBottom;
            float top = anchorTop - _rect.Y1;

            _absoluteOffset = new Rect2D(left, bottom, right, top);
        }

        public void GetAnchors(Rect2D parentRect, out float anchorLeft, out float anchorRight, out float anchorBottom, out float anchorTop)
        {
            anchorLeft = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;
            anchorRight = parentRect.Left + _normalizedAnchoring.X1 * parentRect.Width;
            anchorBottom = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;
            anchorTop = parentRect.Bottom + _normalizedAnchoring.Y1 * parentRect.Height;
        }

        public void Copy(UIRectTransform rectTransform)
        {
            Lock();
            AbsoluteOffset = rectTransform.AbsoluteOffset;
            NormalizedAnchoring = rectTransform.NormalizedAnchoring;
            NormalizedCenter = rectTransform.NormalizedCenter;
            Rect = rectTransform.Rect;
            Unlock(this);
        }
    }
}
