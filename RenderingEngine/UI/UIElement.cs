using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;

namespace RenderingEngine.UI
{
    //TODO: make UIElement actually decent
    public abstract class UIElement
    {
        protected UINode _parent = null;
        public UINode Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }

        protected Rect2D _rect;

        Rect2D _rectOffset;
        Rect2D _anchoring;
        bool _positionSize = false;

        public Rect2D Anchoring { get { return _anchoring; }}
        public Rect2D RectOffset { get { return _rectOffset; } }

        private bool _dirty = true;

        protected bool _isMouseOver;
        protected bool _isMouseDown;

        public Rect2D Rect {
            get {
                return _rect;
            }
        }

        public Rect2D Anchors {
            get {
                return _anchoring;
            }
        }

        public void SetRectOffset(Rect2D pos)
        {
            _positionSize = false;
            _rectOffset = pos;
        }

        public void SetRectPositionSize(float x, float y, float width, float height)
        {
            _positionSize = true;
            _rectOffset = new Rect2D(x, y, width, height);
        }

        public void SetAnchoringOffset(Rect2D anchor)
        {
            _positionSize = false;
            _anchoring = anchor;
        }

        public void SetAnchoringPositionCenter(float x, float y, float centreX = 0.5f, float centreY = 0.5f)
        {
            _positionSize = true;
            _anchoring = new Rect2D(x, y, centreX, centreY);
        }

        public virtual void Update(double deltaTime)
        {
            _isMouseDown = false;
            _isMouseOver = false;
        }

        public virtual void Draw(double deltaTime)
        {
        }

        internal virtual bool ProcessChildEvents()
        {
            return ProcessEvents();
        }

        public virtual bool ProcessEvents()
        {
            if (IsMouseOver())
            {
                _isMouseOver = true;
                if (Input.IsAnyClicked)
                {
                    _isMouseDown = true;
                }
                return true;
            }

            return false;
        }

        public virtual void Resize()
        {
            _dirty = true;
            UpdateRect();
        }

        protected void UpdateRect()
        {
            if (!_dirty)
                return;

            _dirty = false;

            Rect2D parentRect = GetParentRect();

            if (_positionSize)
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
            float bottom =  Y - _anchoring.Y1 * height;
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

            float left = anchorLeft - _rectOffset.X0;
            float right = anchorRight + _rectOffset.X1;
            float top = anchorTop + _rectOffset.Y1;
            float bottom = anchorBottom - _rectOffset.Y0;

            _rect = new Rect2D(left, bottom, right, top);
        }

        private Rect2D GetParentRect()
        {
            Rect2D parentRect;

            if (Parent != null)
            {
                parentRect = Parent.Rect;
            }
            else
            {
                parentRect = Window.Rect;
            }

            return parentRect;
        }

        protected bool IsMouseOver()
        {
            return Intersections.IsInside(Input.MouseX, Input.MouseY, _rect);
        }
    }
}
