using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;

namespace RenderingEngine.UI.Core
{
    public class UIElement
    {
        protected List<UIComponent> _components = new List<UIComponent>();
        protected List<UIElement> _children = new List<UIElement>();
        protected UIRectTransform _rectTransform = new UIRectTransform();

        public Rect2D Rect {
            get { return _rectTransform.Rect; }
        }

        public Rect2D RectOffset {
            get { return _rectTransform.RectOffset; }
        }

        public Rect2D Anchoring {
            get { return _rectTransform.Anchoring; }
        }

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
            for (int i = 0; i < elements.Length; i++)
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

            if (i != -1)
                return (T)_components[i];

            return default;
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

        protected bool _isVisible = true;
        public bool IsVisible { get => _isVisible; set => _isVisible = value; }


        protected bool _dirty = true;


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
            _rectTransform.PositionSize = false;
            _rectTransform.RectOffset = pos;
            _dirty = true;
            return this;
        }

        public UIElement SetRectPositionSize(float x, float y, float width, float height)
        {
            _rectTransform.PositionSize = true;
            _rectTransform.RectOffset = new Rect2D(x, y, width, height);
            _dirty = true;
            return this;
        }

        public UIElement SetAnchoringOffset(Rect2D anchor)
        {
            _rectTransform.PositionSize = false;
            _rectTransform.Anchoring = anchor;
            _dirty = true;
            return this;
        }

        public UIElement SetAnchoringPositionCenter(float x, float y, float centreX = 0.5f, float centreY = 0.5f)
        {
            _rectTransform.PositionSize = true;
            _rectTransform.Anchoring = new Rect2D(x, y, centreX, centreY);
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

        public virtual void Draw(double deltaTime)
        {
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
            _rectTransform.UpdateRect(GetParentRect());

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Resize();
            }
        }

        public virtual void SetDirty()
        {
            _dirty = true;
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
