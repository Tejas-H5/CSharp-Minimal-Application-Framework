using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace MinimalAF {
    public abstract partial class Element {
        protected Element _parent = null;
        protected List<Element> _children = new List<Element>();
        public virtual bool SingleChild => false;

        public bool IsVisibleNextFrame = true;
        protected bool _isVisible = true;
        private bool _shouldTriggerParentResize = false;

        // IDK a better name for this
        public void TriggerLayoutRecalculation() {
            _shouldTriggerParentResize = true;
        }

        private bool _mounted = false;
        internal bool Mounted {
            set {
                _mounted = value;
            }
        }

        public Vector2 Pivot;
        public Vector2 Offset;
        private Rect _relativeRect;
        protected Rect _screenRect; // this is auto-calculated
        bool _rectModified = false;

        public void SetRelativeRect(Rect value) {
            if (RelativeRect != value) {
                RelativeRect = value;
            }
        }

        // TODO: make this somewhat local, or come up with a good way to do that
        protected void SetClearColor(Color4 value) {
            CTX.SetClearColor(value);
        }

        public ArraySlice<Element> Children {
            get {
                return _children;
            }
        }


        public Element this[int index] {
            get {
                return _children[index];
            }
        }

        public Element SetChildren(params Element[] newChildren) {
#if DEBUG
            if (SingleChild && _children != null && _children.Count > 1)
                throw new Exception("This element must only be given 1 child, possibly in the constructor.");
#endif

            // _children will be removed anyway, but this should give O(n) instead of O(n^2)
            for (int i = _children.Count - 1; i >= 0; i--) {
                Remove(i);
            }

            if (newChildren != null) {
                for (int i = 0; i < newChildren.Length; i++) {
                    AddChild(newChildren[i]);
                }
            }

            return this;
        }

        public void RemoveChild(Element child) {
            int index = _children.IndexOf(child);
            if (index == -1) {
                return;
            }

            Remove(index);
        }

        private void Remove(int index) {
            Element child = _children[index];
            _children.RemoveAt(index);
            child.Dismount();
            child._parent = null;

            _shouldTriggerParentResize = true;
        }

        public int Index() {
            if (Parent == null) {
                return 0;
            }

            return Parent._children.IndexOf(this);
        }

        private void Add(Element element) {
            _children.Add(element);

            element.Mount();

            _shouldTriggerParentResize = true;
        }

        public void AddChild(Element element) {
            element.Parent = this;
        }

        public Element Parent {
            get {
                return _parent;
            }
            set {
                if (value == this) {
                    throw new Exception("DevIsStupid exception: An element can't be it's own parent, as this causes infinite recursion");
                }

                if (value == _parent)
                    return;

                if (_parent != null) {
                    _parent.RemoveChild(this);
                }

                _parent = value;

                if (_parent != null) {
                    _parent.Add(this);
                }
            }
        }

        public bool IsVisible {
            get {
                return _isVisible;
            }
            set {
                if (_isVisible == value)
                    return;

                _isVisible = value;
                IsVisibleNextFrame = value;
            }
        }

        public Rect RelativeRect {
            get {
                return _relativeRect;
            }
            set {
                _relativeRect = value;

                if (Parent != null) {
                    Parent.OnChildResize();
                }

                _rectModified = true;
            }
        }

        public float Width {
            get {
                return RelativeRect.Width;
            }
        }
        public float Height {
            get {
                return RelativeRect.Height;
            }
        }


        public Element() {
        }

        /// <summary>
        /// <para>
        /// Gets any Element of type T (or inherited from T) that is the 
        /// closest ancestor of this Element in the UI tree,
        /// including this one.
        /// </para>
        /// <para>
        /// Else it returns null
        /// </para>
        /// <para>
        /// It is simply a linear search that goes up the UI tree and returns an element
        /// if it is of type T or T.GetType().isAssignableFrom returns true
        /// </para>
        /// </summary>
        public T GetAncestor<T>() where T : Element {
            Type tType = typeof(T);
            Element next = this;
            while (next != null) {
                if (next is T || tType.IsAssignableFrom(next.GetType()))
                    return next as T;

                next = next.Parent;
            }

            return null;
        }

        internal void UpdateSelfAndChildren(Rect parentScreenRect) {
            bool shouldBeVisible = IsVisible;
            IsVisible = IsVisibleNextFrame;

            if (!shouldBeVisible) {
                return;
            }

            if (_shouldTriggerParentResize) {
                _shouldTriggerParentResize = false;

                if (Parent != null) {
                    Parent.OnChildResize();
                }
            }

            RecalcScreenRect(parentScreenRect);
            UseCoordinates();

            OnUpdate();

            for (int i = 0; i < _children.Count; i++) {
                _children[i].UpdateSelfAndChildren(_screenRect);
            }

            AfterUpdate();
        }


        internal struct RenderAccumulator {
            public int Depth;
            public Rect ParentScreenRect;

#if DEBUG
            public int HoverDepth;
#endif

            public RenderAccumulator(
                int depth,
                Rect screenRect
#if DEBUG
                , int hoverDepth
#endif
            ) {
                Depth = depth;
                ParentScreenRect = screenRect;

#if DEBUG
                HoverDepth = hoverDepth;
#endif
            }
        }

        internal void RenderSelfAndChildren(Rect parentScreenRect) {
            RenderSelfAndChildren(new RenderAccumulator(
                0,
                parentScreenRect
#if DEBUG
                , 0
#endif
            ));
        }


        private void RecalcScreenRect(Rect parentScreenRect) {
            _screenRect = RelativeRect;
            _screenRect.Move(parentScreenRect.X0 + Offset.X, parentScreenRect.Y0 + Offset.Y);
        }

        public void UseCoordinates() {
            CTX.SetScreenRect(_screenRect);
        }

        internal void RenderSelfAndChildren(RenderAccumulator acc) {
            if (!IsVisible) {
                return;
            }

            RecalcScreenRect(acc.ParentScreenRect);
            UseCoordinates();

            SetTexture(null);

            OnRender();

#if DEBUG
            if (MinimalAFEnvironment.Debug) {
                acc = DrawDebugStuff(acc, _screenRect);
            }
#endif

            for (int i = 0; i < _children.Count; i++) {
                _children[i].RenderSelfAndChildren(new RenderAccumulator(acc.Depth + 1, _screenRect
#if DEBUG
                    , acc.HoverDepth
#endif           
                ));
            }

            AfterRender();
        }

#if DEBUG
        RenderAccumulator DrawDebugStuff(RenderAccumulator acc, Rect newScreenRect) {
            if (!MouseOverSelf)
                return acc;

            SetDrawColor(Color4.RGBA(1, 0, 0, 0.5f));

            RectOutline(2, 1, 1, VW(1) - 1, VH(1) - 1);

            SetDrawColor(Color4.RGBA(1, 0, 0, 0.35f));
            Rect(0, 0, 10, 10);
            Rect(VW(1) - 10, 0, VW(1), 10);
            Rect(0, VH(1) - 10, 10, VH(1));
            Rect(VW(1) - 10, VH(1) - 10, VW(1), VH(1));

            SetDrawColor(Color4.RGB(0, 0, 0));
            int textSize = 12;
            SetFont("Consolas", textSize);

            string text = GetType().Name + " " + RelativeRect.ToString() + " Depth: " + acc.Depth;
            Text(text, -newScreenRect.X0 + 10, -newScreenRect.Y0 + 10 + textSize * acc.HoverDepth);

            SetFont("");
            SetTexture(null);

            acc.HoverDepth += 1;
            return acc;
        }
#endif

        public void Mount() {
            ApplicationWindow w = GetAncestor<ApplicationWindow>();

            if (w == null) {
                return;
            }

            Window mockWindow = GetAncestor<Window>();
            Mount(mockWindow);
        }

        private void Mount(Window w) {
            OnMount(w);

            for (int i = 0; i < _children.Count; i++) {
                _children[i].Mount(w);
            }

            AfterMount(w);

            _mounted = true;
        }


        public void Dismount() {
            for (int i = 0; i < _children.Count; i++) {
                _children[i].Dismount();
            }

            OnDismount();
            _mounted = false;
        }


        public void Layout() {
            if (!_mounted) {
                return;
            }

            _onChildResizeLock = true;

            for (int i = 0; i < _children.Count; i++) {
                if (_children[i]._rectModified)
                    continue;

                _children[i].RelativeRect = DefaultRect();
            }

            OnLayout();

            for (int i = 0; i < _children.Count; i++) {
                _children[i]._rectModified = false;
            }

            _onChildResizeLock = false;
        }

        /// <summary>
        /// <para>
        /// This is how the parent will size this rect by default. The parent will call this method on it's elements
        /// before calling OnLayout
        /// </para>
        /// 
        /// <para>
        /// Note that I have never ever had to use this (and when I tried, I later realised my design was wrong), 
        /// and if you are using it, it may be a problem with your design.
        /// I am keeping it in for now, just in case
        /// </para>
        /// </summary>
        public virtual Rect DefaultRect() {
            return new Rect(0, 0, VW(1), VH(1));
        }


        /// <summary>
        /// <para>
        /// Size and position your child elements in this method.
        /// After this method has been called, all child elements must be correctly sized and positioned, and this
        /// element must be correctly sized (you should assume that the parent element will position this element).
        /// </para>
        /// 
        /// <para>
        /// Don't forget to either call <see cref="LayoutChildren"/>, or alternatively call <see cref="Layout"/> 
        /// at least once on all the children. 
        /// 
        /// This is not done by default, to allow for more flexibility.
        /// </para>
        /// </summary>
        public virtual void OnLayout() {
            LayoutChildren();
        }

        protected void LayoutChildren() {
            for (int i = 0; i < _children.Count; i++) {
                _children[i].Layout();
            }
        }


        bool _onChildResizeLock = false;
        void OnChildResize() {
            _shouldTriggerParentResize = false;

            if (_onChildResizeLock) {
                return;
            }

            Rect rect = RelativeRect;

            Layout();

            bool sizeChanged = rect != RelativeRect;
            if (Parent != null && sizeChanged) {
                Parent.OnChildResize();
            }
        }

        /// <summary>
        /// <para>
        /// Is called when this element is added to the main element tree, and before it's children
        /// have mounted
        /// </para>
        /// </summary>
        public virtual void OnMount(Window w) {

        }


        /// <summary>
        /// Is called after all children have mounted
        /// </summary>
        /// <param name="w"></param>
        public virtual void AfterMount(Window w) {

        }

        /// <summary>
        /// The update cycle that runs every frame.
        /// Frequency is controlled by the window configuration's UpdateFrequency
        /// </summary>
        public virtual void OnUpdate() {

        }

        public virtual void AfterUpdate() {

        }

        /// <summary>
        /// Render cycle that runs every frame.
        /// Frequency is controlled by the window configuration's RenderFrequency.
        /// </summary>
        public virtual void OnRender() {

        }

        public virtual void AfterRender() {

        }



        /// <summary>
        /// Implement cleanup code here. This is ran in children before it is ran in the parents
        /// </summary>
        public virtual void OnDismount() {

        }
    }
}
