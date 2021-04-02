using OpenTK.Windowing.GraphicsLibraryFramework;
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
        UIElement _parent = null;
        public UIElement Parent {
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

        protected List<UIElement> _children = new List<UIElement>();


        public void AddChild(UIElement element)
        {
            _children.Add(element);
            element.Parent = this;
        }

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

        public virtual void Update(double deltaTime, GraphicsWindow window)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Update(deltaTime, window);
            }
        }

        public virtual void Draw(double deltaTime)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Draw(deltaTime);
            }
        }

        public virtual void Resize(GraphicsWindow window)
        {
            _dirty = true;
            UpdateRect(window);

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Resize(window);
            }
        }

        protected void UpdateRect(GraphicsWindow window)
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
                parentRect = window.Rect;
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

        protected bool IsMouseOver(GraphicsWindow window)
        {
            return Intersections.IsInside(Input.MouseX, Input.MouseY, _rect);
        }
    }
}
