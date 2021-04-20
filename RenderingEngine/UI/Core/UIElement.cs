using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Components.MouseInput;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RenderingEngine.UI.Core
{
    public class UIElement
    {
        protected List<UIComponent> _components = new List<UIComponent>();
        protected List<UIElement> _children = new List<UIElement>();
        protected UIRectTransform _rectTransform = new UIRectTransform();

        /// <summary>
        /// Should not be used over the other wrapped getters/setters if possible.
        /// mainly for components to make some modification for example after a Resize()
        /// </summary>
        public UIRectTransform RectTransform { get { return _rectTransform; } }

        public UIElement()
        {
            SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1));
            SetAbsoluteOffset(new Rect2D(0, 0, 0, 0));

            _rectTransform.OnDataChanged += _rectTransform_OnDataChanged;
        }

        private void _rectTransform_OnDataChanged(UIRectTransform obj)
        {
            _dirty = true;
        }

        public Rect2D Rect {
            get { return _rectTransform.Rect; }
            set {
                _rectTransform.Rect = value;
            }
        }

        public float AbsCenterX {
            get {
                return Rect.X0 + _rectTransform.NormalizedCenter.X * Rect.Width;
            }
        }

        public float AbsCenterY {
            get {
                return Rect.Y0 + _rectTransform.NormalizedCenter.Y * Rect.Height;
            }
        }

        public PointF AnchoredPositionAbs {
            get {
                return _rectTransform.AnchoredPositionAbs;
            }
        }

        public PointF NormalizedCenter {
            get { return _rectTransform.NormalizedCenter; }
            set { _rectTransform.NormalizedCenter = value; }
        }

        public Rect2D AbsoluteOffset {
            get { return _rectTransform.AbsoluteOffset; }
        }

        public Rect2D NormalizedAnchoring {
            get { return _rectTransform.NormalizedAnchoring; }
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
            AddChildVirtual(element);

            return this;
        }

        protected virtual void AddChildVirtual(UIElement element)
        {
            _children.Add(element);
            element.Parent = this;
            element.Resize();
        }

        public void RemoveChild(UIElement element)
        {
            if (_children.Contains(element))
            {
                //Not sure what to do with this. will it just get deleted automatically?
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

        public void RemoveAllChildren()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Parent = null;
            }
            _children.Clear();
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

        /// <summary>
        /// Gets a component of class T or a subclass of T from the component list. 
        /// </summary>
        /// <typeparam name="T">The type of class that is wanted. 
        /// must derive from RenderingEngine.UI.Core.UIComponent.</typeparam>
        /// <returns></returns>
        public T GetComponentOfType<T>() where T : UIComponent
        {
            int i = ComponentOfTypeIndex(typeof(T));

            if (i != -1)
                return (T)_components[i];

            return default;
        }

        /// <summary>
        /// same as GetComponentOfType but does a depth first search in all children
        /// </summary>
        public T GetComponentInChildrenOfType<T>() where T : UIComponent
        {
            T res = GetComponentOfType<T>();
            if (res != null)
                return res;

            for(int i = 0; i < _children.Count; i++)
            {
                res = _children[i].GetComponentInChildrenOfType<T>();
                if (res != null)
                    return res;
            }

            return null;
        }

        /// <summary>
        /// Adds a component to the list of components.
        /// Generic, because two components of the same type T may not be added to the list.
        /// This is to ensure the correct behaviour of GetComponent.
        /// Most of the time, you don't need multiple of the same component anyway,
        /// and a 'mulit-component' (i.e a component that manages multiple of some component
        /// in a List or similar) can be created if you do.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comp"></param>
        /// <returns></returns>
        public UIElement AddComponent<T>(T comp) where T : UIComponent
        {
            return InsertComponent(_components.Count, comp);
        }

        public UIElement InsertComponent<T>(int index, T comp) where T : UIComponent
        {
            int i = ComponentOfTypeIndex(comp.GetType());

            if (i == -1)
            {
                _components.Insert(index, comp);
                comp.SetParent(this);
#if DEBUG
                SetParentDebug();
#endif
                return this;
            }

            throw new Exception("A component of this type already exists");
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

        private bool _isVisible = true;
        public bool IsVisible {
            get { return _isVisible; }
            set {
                _isVisible = value;
                IsVisibleNextFrame = value;
            }
        }

        public bool IsVisibleNextFrame = true;

        protected bool _dirty = true;

#if DEBUG
        UIMouseListener _mouseListenComponent;
        void SetParentDebug()
        {
            _mouseListenComponent = GetComponentOfType<UIMouseListener>();
        }
#endif

        public UIElement SetAbsOffsetsX(float left, float right)
        {
            _rectTransform.SetAbsOffsetsX(left, right);
            return this;
        }

        public UIElement SetAbsOffsetsY(float bottom, float top)
        {
            _rectTransform.SetAbsOffsetsY(bottom, top);
            return this;
        }

        public UIElement SetAbsoluteOffset(float offset)
        {
            return SetAbsoluteOffset(new Rect2D(offset, offset, offset, offset));
        }

        public UIElement SetAbsoluteOffset(Rect2D pos)
        {
            _rectTransform.SetAbsoluteOffset(pos);
            return this;
        }

        public UIElement SetAbsPositionSizeX(float x, float width)
        {
            _rectTransform.SetAbsPositionSizeX(x, width);
            return this;
        }

        public UIElement SetAbsPositionSizeY(float y, float height)
        {
            _rectTransform.SetAbsPositionSizeY(y, height);
            return this;
        }

        public UIElement SetAbsPositionSize(float x, float y, float width, float height)
        {
            _rectTransform.SetAbsPositionSize(x, y, width, height);
            return this;
        }

        public UIElement SetNormalizedAnchoringX(float left, float right)
        {
            _rectTransform.SetNormalizedAnchoringX(left, right);
            return this;
        }

        public UIElement SetNormalizedAnchoringY(float bottom, float top)
        {
            _rectTransform.SetNormalizedAnchoringY(bottom, top);
            return this;
        }


        public UIElement SetNormalizedAnchoring(Rect2D anchor)
        {
            _rectTransform.SetNormalizedAnchoring(anchor);
            return this;
        }

        public UIElement SetNormalizedPositionCenterX(float x, float centreX = 0.5f)
        {
            _rectTransform.SetNormalizedPositionCenterX(x, centreX);
            return this;
        }

        public UIElement SetNormalizedPositionCenterY(float y, float centreY = 0.5f)
        {
            _rectTransform.SetNormalizedPositionCenterY(y, centreY);
            return this;
        }

        public UIElement SetNormalizedPositionCenter(float x, float y, float centreX = 0.5f, float centreY = 0.5f)
        {
            _rectTransform.SetNormalizedPositionCenter(x, y, centreX, centreY);
            return this;
        }

        public UIElement SetNormalizedCenter(float centerX = 0.5f, float centerY = 0.5f)
        {
            _rectTransform.NormalizedCenter = new PointF(centerX, centerY);
            return this;
        }

        public void Update(double deltaTime)
        {
            if (!IsVisible)
            {
                return;
            }

            IsVisible = IsVisibleNextFrame;

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

        public void DrawIfVisible(double deltaTime)
        {
            if (!IsVisible)
            {
                return;
            }

            BeforeDraw(deltaTime);

            Draw(deltaTime);
            DrawChildren(deltaTime);

            AfterDraw(deltaTime);
#if DEBUG
            if (UICreator.Debug)
            {
                DrawDebug();
            }
#endif
        }

        protected virtual void DrawChildren(double deltaTime)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].DrawIfVisible(deltaTime);
            }
        }

#if DEBUG
        void DrawDebug()
        {
            //CTX.SetDrawColor(0, 0, 0, 0.5f);
            //CTX.DrawRectOutline(1, Rect);

            if (_mouseListenComponent != null)
            {
                if (_mouseListenComponent.IsProcessingEvents)
                {
                    CTX.SetDrawColor(1, 0, 0, 1);
                    CTX.DrawRect(Rect.X0 + 5, Rect.Y1 - 5, Rect.X0 + 15, Rect.Y1 - 15);
                }
            }
        }
#endif

        public void BeforeDraw(double deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].BeforeDraw(deltaTime);
            }
        }

        public virtual void Draw(double deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Draw(deltaTime);
            }
        }

        public void AfterDraw(double deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].AfterDraw(deltaTime);
            }
        }

        internal virtual bool ProcessChildEvents()
        {
            bool hasProcessed = false;
            for (int i = 0; i < _children.Count; i++)
            {
                // Fun fact:
                // if I write this as hasProcessed || _children[i].ProcessChildEvents(), then 
                // ProcessChildEvents() half of the or statement won't execute if hasProcessed is true

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

        public void UpdateRect()
        {
            _rectTransform.UpdateRect(GetParentRect());
        }

        public void Resize()
        {
            _rectTransform.Lock();

            UpdateRect();

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnResize();
            }

            ResizeChildren();

            _rectTransform.UnlockNonInvoking();
        }

        public virtual void ResizeChildren()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Resize();
            }
        }

        public virtual void SetDirty()
        {
            _dirty = true;
        }

        public Rect2D GetParentRect()
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
