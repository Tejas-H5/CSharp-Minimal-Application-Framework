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
        UINode _parent = null;
        public UINode Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }

        protected Rect2D _rect;
        protected Rect2D _rectOffset;
        protected Rect2D _anchoring;
        private bool _dirty = true;

        protected bool _isMouseOver;

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
            _rectOffset = pos;
        }

        public void SetAnchoring(Rect2D anchor)
        {
            _anchoring = anchor;
        }

        public virtual void Update(double deltaTime)
        {
            _isMouseOver = IsMouseOver();
        }

        public virtual void Draw(double deltaTime)
        {
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

            Rect2D parentRect;

            if (Parent != null)
            {
                parentRect = Parent.Rect;
            }
            else
            {
                parentRect = Window.Rect;
            }

            float anchorLeft = parentRect.X0 + _anchoring.X0 * parentRect.Width;
            float anchorRight = parentRect.X0 + _anchoring.X1 * parentRect.Width;
            float anchorTop = parentRect.Y0 + _anchoring.Y1 * parentRect.Height;
            float anchorBottom = parentRect.Y0 + _anchoring.Y0 * parentRect.Height;

            float left = anchorLeft - _rectOffset.X0;
            float right = anchorRight + _rectOffset.X1;
            float top = anchorTop + _rectOffset.Y1;
            float bottom = anchorBottom - _rectOffset.Y0;

            _rect = new Rect2D(left, bottom, right, top);
        }

        protected bool IsMouseOver()
        {
            return Intersections.IsInside(Input.MouseX, Input.MouseY, _rect);
        }
    }
}
