using MinimalAF.Util;
using System;
using System.Drawing;

namespace MinimalAF {
    /// <summary>
    /// Inspired by Unity, because I liked using it and I missed it when I went to other frameworks.
    /// I don't know if this is exactly how they implemented it though.
    /// 
    /// Doesn't matter, I am sticking with this one for now as it suits my needs more.
    /// </summary>
    public class RectTransform {
        Rect2D _rect;
        Rect2D _absoluteOffset;
        Rect2D _normalizedAnchoring;
        PointF _normalizedCenter;

        public RectTransform() {
            Anchors(new Rect2D(0, 0, 1, 1));
            Offsets(new Rect2D(0, 0, 0, 0));
        }

        public RectTransform(RectTransform other) {
            Copy(other);
            _absoluteOffset = other._absoluteOffset;
            _normalizedAnchoring = other._normalizedAnchoring;
            _normalizedCenter = other._normalizedCenter;
            _rect = other._rect;
        }

        public void Copy(RectTransform rectTransform) {
            AbsoluteOffset = rectTransform.AbsoluteOffset;
            NormalizedAnchoring = rectTransform.NormalizedAnchoring;
            NormalizedCenter = rectTransform.NormalizedCenter;
            Rect = rectTransform.Rect;
        }

        public Rect2D Rect {
            get {
                return _rect;
            }
            set {
                _rect = value;
            }
        }

        public PointF NormalizedCenter {
            get {
                return _normalizedCenter;
            }
            set {
                _normalizedCenter = value;
            }
        }

        public Rect2D AbsoluteOffset {
            get {
                return _absoluteOffset;
            }
            set {
                _absoluteOffset = value;
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
            get {
                return _normalizedAnchoring;
            }
            set {
                _normalizedAnchoring = value;
                _normalizedAnchoring.X0 = MathUtilF.Clamp01(_normalizedAnchoring.X0);
                _normalizedAnchoring.X1 = MathUtilF.Clamp01(_normalizedAnchoring.X1);
                _normalizedAnchoring.Y0 = MathUtilF.Clamp01(_normalizedAnchoring.Y0);
                _normalizedAnchoring.Y1 = MathUtilF.Clamp01(_normalizedAnchoring.Y1);

                _normalizedAnchoring.X1 = MathF.Max(_normalizedAnchoring.X0, _normalizedAnchoring.X1);
                _normalizedAnchoring.Y1 = MathF.Max(_normalizedAnchoring.Y0, _normalizedAnchoring.Y1);
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

        public float AbsCenterX {
            get {
                return Rect.X0 + NormalizedCenter.X * Rect.Width;
            }
        }

        public float AbsCenterY {
            get {
                return Rect.Y0 + NormalizedCenter.Y * Rect.Height;
            }
        }

        public void OffsetsX(float left, float right) {
            _absoluteOffset.X0 = left;
            _absoluteOffset.X1 = right;
        }

        public void OffsetsY(float bottom, float top) {
            _absoluteOffset.Y0 = bottom;
            _absoluteOffset.Y1 = top;
        }

        public void Offsets(float offset) {
            Offsets(new Rect2D(offset, offset, offset, offset));
        }

        public void Offsets(Rect2D rectOffset) {
            AbsoluteOffset = rectOffset;
        }

        public void PosSizeX(float x, float width) {
            _absoluteOffset.X0 = x - _normalizedCenter.X * width;
            _absoluteOffset.X1 = -x - ((1.0f - _normalizedCenter.X) * width);
        }

        public void PosSizeY(float y, float height) {
            _absoluteOffset.Y0 = y - _normalizedCenter.Y * height;
            _absoluteOffset.Y1 = -y - ((1.0f - _normalizedCenter.Y) * height);
        }

        public void PosSize(float x, float y, float width, float height) {
            PosSizeX(x, width);
            PosSizeY(y, height);
        }

        public void AnchorsX(float left, float right) {
            _normalizedAnchoring.X0 = left;
            _normalizedAnchoring.X1 = right;

            NormalizedAnchoring = _normalizedAnchoring;
        }

        public void AnchorsY(float bottom, float top) {
            _normalizedAnchoring.Y0 = bottom;
            _normalizedAnchoring.Y1 = top;

            NormalizedAnchoring = _normalizedAnchoring;
        }

        public void Anchors(Rect2D anchor) {
            NormalizedAnchoring = anchor;
        }

        public void AnchoredPos(float x, float y) {
            NormalizedAnchoring = new Rect2D(x, y, x, y);
        }

        public void AnchoredCenter(float x = 0.5f, float y = 0.5f) {
            NormalizedCenter = new PointF(x, y);
        }

        public void AnchoredPosX(float x) {
            NormalizedAnchoring = new Rect2D(x, _normalizedAnchoring.Y0, x, _normalizedAnchoring.Y1);
        }

        public void AnchoredPosY(float y) {
            NormalizedAnchoring = new Rect2D(_normalizedAnchoring.X0, y, _normalizedAnchoring.X1, y);
        }

        public void AnchoredCenterX(float x) {
            NormalizedCenter = new PointF(x, _normalizedCenter.Y);
        }

        public void AnchoredCenterY(float y) {
            NormalizedCenter = new PointF(_normalizedCenter.X, y);
        }

        public void UpdateRectFromOffset(Rect2D parentRect) {
            float anchorLeft, anchorRight, anchorBottom, anchorTop;
            GetAnchors(parentRect, out anchorLeft, out anchorRight, out anchorBottom, out anchorTop);

            float left = anchorLeft + _absoluteOffset.X0;
            float right = anchorRight - _absoluteOffset.X1;
            float bottom = anchorBottom + _absoluteOffset.Y0;
            float top = anchorTop - _absoluteOffset.Y1;

            _rect = new Rect2D(left, bottom, right, top);
        }

        public void UpdateOffsetFromRect(Rect2D parentRect) {
            float anchorLeft, anchorRight, anchorBottom, anchorTop;
            GetAnchors(parentRect, out anchorLeft, out anchorRight, out anchorBottom, out anchorTop);

            float left = _rect.X0 - anchorLeft;
            float right = anchorRight - _rect.X1;
            float bottom = _rect.Y0 - anchorBottom;
            float top = anchorTop - _rect.Y1;

            _absoluteOffset = new Rect2D(left, bottom, right, top);
        }

        public void GetAnchors(Rect2D parentRect, out float anchorLeft, out float anchorRight, out float anchorBottom, out float anchorTop) {
            anchorLeft = parentRect.Left + _normalizedAnchoring.X0 * parentRect.Width;
            anchorRight = parentRect.Left + _normalizedAnchoring.X1 * parentRect.Width;
            anchorBottom = parentRect.Bottom + _normalizedAnchoring.Y0 * parentRect.Height;
            anchorTop = parentRect.Bottom + _normalizedAnchoring.Y1 * parentRect.Height;
        }


        public void SetWidth(float newWidth) {
            float centerX = NormalizedCenter.X;
            float deltaW = Width - newWidth;
            _rect.X0 += deltaW * centerX;
            _rect.X1 -= deltaW * (1.0f - centerX);
        }

        public void SetHeight(float newHeight) {
            float centerY = NormalizedCenter.Y;
            float deltaH = Height - newHeight;
            _rect.Y0 += deltaH * centerY;
            _rect.Y1 -= deltaH * (1.0f - centerY);
        }
    }
}
