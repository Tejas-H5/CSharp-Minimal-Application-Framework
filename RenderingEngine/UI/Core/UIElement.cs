using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;

namespace RenderingEngine.UI
{
    public class UIElement
    {
        protected List<UIComponent> _components = new List<UIComponent>();
        protected List<UIElement> _children = new List<UIElement>();

        public UIElement this[int index] {
            get {
                return _children[index];
            }
        }

        public int Count {
            get {
                return _children.Count;
            }
        }

        public UIElement AddChildren(params UIElement[] elements)
        {
            for(int i = 0; i < elements.Length; i++)
            {
                AddChild(elements[i]);
            }
            return this;
        }

        public UIElement AddChild(UIElement element)
        {
            _children.Add(element);
            element.Parent = this;
            element.Resize();
            return this;
        }

        public void RemoveChild(UIElement element)
        {
            if (_children.Contains(element))
            {
                _children.Remove(element);
                element.Parent = null;
            }
        }

        public void RemoveChild(int index)
        {
            if (index >= 0 && index < _children.Count)
            {
                _children[index].Parent = null;
                _children.RemoveAt(index);
            }
        }


        private int ComponentOfTypeIndex(Type t)
        {
            int index = -1;
            for (int i = 0; i < _components.Count; i++)
            {
                Type ti = _components[i].GetType();
                if (ti.IsSubclassOf(t) || ti == t)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }


        public T GetComponentOfType<T>() where T : UIComponent
        {
            int i = ComponentOfTypeIndex(typeof(T));

            if(i != -1)
                return (T)(_components[i]);

            return default(T);
        }

        public UIElement AddComponent<T>(T comp) where T : UIComponent
        {
            int i = ComponentOfTypeIndex(comp.GetType());

            //var t = comp.GetType();
            //var t2 = typeof(T);

            if (i == -1)
            {
                _components.Add(comp);
                comp.SetParent(this);
                return this;
            }

            return null;
        }

        
        protected UIElement _parent = null;
        public UIElement Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
                _dirty = true;
            }
        }

        protected Rect2D _rect;

        Rect2D _rectOffset;
        Rect2D _anchoring;
        bool _positionSize = false;

        public Rect2D Anchoring { get { return _anchoring; } }
        public Rect2D RectOffset { get { return _rectOffset; } }

        protected bool _isVisible = true;
        public bool IsVisible { get => _isVisible; set => _isVisible = value; }


        protected bool _dirty = true;

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

        public UIElement()
        {
            SetAnchoringOffset(new Rect2D(0, 0, 1, 1));
            SetRectOffset(new Rect2D(0, 0, 0, 0));
        }

        public UIElement SetRectOffset(float offset)
        {
            return SetRectOffset(new Rect2D(offset, offset, offset, offset));
        }

        public UIElement SetRectOffset(Rect2D pos)
        {
            _positionSize = false;
            _rectOffset = pos;
            _dirty = true;
            return this;
        }

        public UIElement SetRectPositionSize(float x, float y, float width, float height)
        {
            _positionSize = true;
            _rectOffset = new Rect2D(x, y, width, height);
            _dirty = true;
            return this;
        }

        public UIElement SetAnchoringOffset(Rect2D anchor)
        {
            _positionSize = false;
            _anchoring = anchor;
            _dirty = true;
            return this;
        }

        public UIElement SetAnchoringPositionCenter(float x, float y, float centreX = 0.5f, float centreY = 0.5f)
        {
            _positionSize = true;
            _anchoring = new Rect2D(x, y, centreX, centreY);
            _dirty = true;
            return this;
        }

        public virtual void Update(double deltaTime)
        {
            if (_dirty)
            {
                _dirty = false;
                Resize();
            }

            UpdateChildren(deltaTime);

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Update(deltaTime);
            }

            if (_parent == null)
            {
                ProcessChildEvents();
            }
        }

        public virtual void UpdateChildren(double deltaTime)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Update(deltaTime);
            }
        }

        public virtual void DrawIfVisible(double deltaTime)
        {
            if (!_isVisible)
                return;

            Draw(deltaTime);

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].DrawIfVisible(deltaTime);
            }
        }

        public virtual void Draw(double deltaTime) {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Draw(deltaTime);
            }
        }

        internal virtual bool ProcessChildEvents()
        {
            bool hasProcessed = false;
            for (int i = 0; i < _children.Count; i++)
            {
                // Fun fact:
                // if I write this as hasProcessed || _children[i].ProcessChildEvent(), then the second
                // half of the or statement won't execute if hasProcessed is true

                bool hasChildProcessed = _children[i].ProcessChildEvents();
                hasProcessed = hasProcessed || hasChildProcessed;
            }

           if (hasProcessed)
                return true;

            return ProcessComponentEvents();
        }

        public virtual bool ProcessComponentEvents()
        {
            bool res = false;
            for (int i = 0; i < _components.Count; i++)
            {
                bool componentRes = _components[i].ProcessEvents();
                res = res || componentRes;
            }

            return res;
        }

        public virtual void Resize()
        {
            UpdateRect();

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Resize();
            }
        }

        public virtual void SetDirty()
        {
            _dirty = true;
        }

        protected void UpdateRect()
        {
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
    }
}
