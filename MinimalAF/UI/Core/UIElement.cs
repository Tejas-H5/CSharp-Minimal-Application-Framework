using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.UI
{
    public class UIElement
    {
        public List<UIComponent> Components { get { return _components; } }
        protected List<UIComponent> _components = new List<UIComponent>();

        /// <summary>
        /// Should not be used over the other wrapped getters/setters if possible.
        /// mainly for components to make some modification for example after a Resize()
        /// </summary>
        public UIRectTransform RectTransform { get { return _rectTransform; } }
        protected UIRectTransform _rectTransform = new UIRectTransform();

        protected List<UIElement> _children = new List<UIElement>();

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

        public bool IsVisible {
            get { return _isVisible; }
            set {
                _isVisible = value;
                IsVisibleNextFrame = value;
                if (_parent != null)
                {
                    _parent.SetDirty();
                }
            }
        }
        private bool _isVisible = true;

        public bool IsVisibleNextFrame = true;
        protected bool _dirty = true;

        public UIElement()
        {
            Anchors(new Rect2D(0, 0, 1, 1));
            Offsets(new Rect2D(0, 0, 0, 0));

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
            if (element == null)
                return this;

            if (element.Parent == this)
                return this;

            if (element.Parent != null)
            {
                element.Parent.RemoveChild(element);
            }

            AddChildVirtual(element);
            _dirty = true;
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

        protected int ComponentOfTypeIndex(Type t)
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

        protected bool ChildContainsComponentOfType(Type t)
        {
            int componentIndex = ComponentOfTypeIndex(t);
            if (componentIndex != -1)
                return true;

            for (int i = 0; i < _children.Count; i++)
            {
                bool res = _children[i].ChildContainsComponentOfType(t);
                if (res)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// same as GetComponentOfType but does a depth first search in all children
        /// </summary>
        public T GetComponentInChildrenOfType<T>() where T : UIComponent
        {
            T res = GetComponentOfType<T>();
            if (res != null)
                return res;

            for (int i = 0; i < _children.Count; i++)
            {
                res = _children[i].GetComponentInChildrenOfType<T>();
                if (res != null)
                    return res;
            }

            return null;
        }

        /// <summary>
        /// Calls AddComponent(x) for each x in list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public UIElement AddComponents(params UIComponent[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                AddComponent(list[i]);
            }
            return this;
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
            Type componentType = comp.GetType();

            try
            {
                EnsureRequiredComponents(componentType);

                EnsureRequiredComponentsInChildren(componentType);
            }
            catch (Exception e)
            {
                throw e;
            }

            int componentIndex = ComponentOfTypeIndex(componentType);

            if (componentIndex != -1)
            {
                throw new Exception("A component of this type already exists");
            }

            _components.Insert(index, comp);
            comp.SetParent(this);

#if DEBUG
            SetParentDebug();
#endif

            return this;
        }

        private void EnsureRequiredComponents(Type componentType)
        {
            RequiredComponents requiredComponents = (RequiredComponents)Attribute.GetCustomAttribute(componentType, typeof(RequiredComponents));
            if (requiredComponents != null)
            {
                for (int i = 0; i < requiredComponents.ComponentTypes.Length; i++)
                {
                    int existingIndex = ComponentOfTypeIndex(requiredComponents.ComponentTypes[i]);
                    if (existingIndex == -1)
                        throw new Exception("This component requires the following other components: " +
                            $"{requiredComponents.GetComponentListString()}");
                }
            }
        }

        private void EnsureRequiredComponentsInChildren(Type componentType)
        {
            RequiredComponentsInChildren requiredComponents = (RequiredComponentsInChildren)Attribute.GetCustomAttribute(componentType, typeof(RequiredComponentsInChildren));
            if (requiredComponents != null)
            {
                for (int i = 0; i < requiredComponents.ComponentTypes.Length; i++)
                {
                    Type requiredType = requiredComponents.ComponentTypes[i];
                    bool res = ChildContainsComponentOfType(requiredType);
                    if (!res)
                        throw new Exception("This component requires the following other components in itself or its children: " +
                            $"{requiredComponents.GetComponentListString()}");
                }
            }
        }

#if DEBUG
        UIMouseListener _mouseListenComponent;
        void SetParentDebug()
        {
            _mouseListenComponent = GetComponentOfType<UIMouseListener>();
        }

        public bool HasDebugComponent()
        {
            return (GetComponentOfType<UIDebugComponent>() != null);
        }
#endif

        public UIElement OffsetsX(float left, float right)
        {
            _rectTransform.OffsetsX(left, right);
            return this;
        }

        public UIElement OffsetsY(float bottom, float top)
        {
            _rectTransform.OffsetsY(bottom, top);
            return this;
        }

        public UIElement Offsets(float offset)
        {
            return Offsets(new Rect2D(offset, offset, offset, offset));
        }

        public UIElement Offsets(Rect2D pos)
        {
            _rectTransform.SetAbsoluteOffset(pos);
            return this;
        }

        public UIElement Offsets(float left, float bottom, float right, float top)
        {
            return Offsets(new Rect2D(left, bottom, right, top));
        }

        public UIElement PosSizeX(float x, float width)
        {
            _rectTransform.PosSizeX(x, width);
            return this;
        }

        public UIElement PosSizeY(float y, float height)
        {
            _rectTransform.PosSizeY(y, height);
            return this;
        }

        public UIElement Pos(float x, float y)
        {
            _rectTransform.PosSize(x, y, _rectTransform.Width, _rectTransform.Height);
            return this;
        }

        public UIElement PosX(float x)
        {
            _rectTransform.PosSizeX(x, _rectTransform.Width);
            return this;
        }

        public UIElement PosY(float y)
        {
            _rectTransform.PosSizeY(y, _rectTransform.Height);
            return this;
        }

        public UIElement PosSize(float x, float y, float width, float height)
        {
            _rectTransform.PosSize(x, y, width, height);
            return this;
        }

        public UIElement AnchorsX(float left, float right)
        {
            _rectTransform.AnchorsX(left, right);
            return this;
        }

        public UIElement AnchorsY(float bottom, float top)
        {
            _rectTransform.AnchorsY(bottom, top);
            return this;
        }


        public UIElement Anchors(Rect2D anchor)
        {
            _rectTransform.Anchors(anchor);
            return this;
        }

        public UIElement Anchors(float left, float bottom, float right, float top)
        {
            _rectTransform.Anchors(new Rect2D(left, bottom, right, top));
            return this;
        }

        public UIElement AnchoredPosX(float x)
        {
            _rectTransform.AnchoredPosX(x);
            return this;
        }

        public UIElement AnchoredPosY(float y)
        {
            _rectTransform.AnchoredPosY(y);
            return this;
        }

        public UIElement AnchoredCenterX(float x)
        {
            _rectTransform.AnchoredCenterX(x);
            return this;
        }

        public UIElement AnchoredCenterY(float y)
        {
            _rectTransform.AnchoredCenterY(y);
            return this;
        }

        public UIElement AnchoredPos(float x, float y)
        {
            _rectTransform.AnchoredPos(x, y);
            return this;
        }

        //Set this before you set the anchored position
        public UIElement AnchoredCenter(float x = 0.5f, float y = 0.5f)
        {
            _rectTransform.AnchoredCenter(x, y);
            return this;
        }

        public UIElement AnchoredPosCenter(float x, float y, float centerX = 0.5f, float centerY = 0.5f)
        {
            AnchoredCenter(centerX, centerY);
            AnchoredPos(x, y);
            return this;
        }

        public UIElement AnchoredPosCenterX(float x, float centerX)
        {
            AnchoredCenterX(centerX);
            AnchoredPosX(x);
            return this;
        }

        public UIElement AnchoredPosCenterY(float y, float centerY)
        {
            AnchoredCenterY(centerY);
            AnchoredPosY(y);
            return this;
        }

        public void Update(double deltaTime)
        {
            if (!IsVisible)
            {
                IsVisible = IsVisibleNextFrame;
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
        private void DrawDebug()
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
            _rectTransform.UpdateRectFromOffset(GetParentRect());
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

            _rectTransform.Unlock(_rectTransform);

            _dirty = false;
        }

        public virtual void ResizeChildren()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Resize();
            }

            _dirty = false;
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

        protected virtual UIElement ShallowCopy()
        {
            UIElement copy = new UIElement();
            copy.RectTransform.Copy(RectTransform);
            return copy;
        }

        public UIElement DeepCopy()
        {
            UIElement duplicate = ShallowCopy();
            UIComponent[] components = _components.ToArray();
            for (int i = 0; i < components.Length; i++)
            {
                components[i] = components[i].Copy();
            }

            duplicate.AddComponents(
                components
            );

            for (int i = 0; i < _children.Count; i++)
            {
                duplicate.AddChild(_children[i].DeepCopy());
            }

            return duplicate;
        }
    }
}
