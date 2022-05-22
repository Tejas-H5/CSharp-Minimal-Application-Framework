using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace MinimalAF {
    public abstract partial class Element {
        private readonly List<Element> _children = new List<Element>();

        private Element _parent = null;
        private int _stackingOffset = 0;
        private bool _mounted = false,
            _isVisibleNextFrame = true,
            _isVisible = true,
            _rectModified,
            _shouldTriggerParentResize = false,
            _clipping = false;

        private Vector2 _pivot, _offset;
        private Rect _relativeRect, _screenRect, _defaultScreenRect;

#if DEBUG
        private bool _debug = false;
#endif

        protected Element() {
        }

        /// <summary>
        /// Vector between 0 and 1 defining how this rectangle pivots. A pivot of 0,0 means that RelativeRect is sized and positioned
        /// relative to 
        /// </summary>
        public ref Vector2 Pivot => ref _pivot;

        /// <summary>
        /// An offset that moves this rectangle without triggering a layout update.
        /// Usefull for animating dragging, and 
        /// </summary>
        public ref Vector2 Offset => ref _offset;

        protected void ConstrainOffsetToParent() {
            var constrainRect = Rect.PivotSize(Parent.Width, Parent.Height, Pivot.X, Pivot.Y);

            constrainRect.X1 -= Width * (1f - Pivot.X);
            constrainRect.Y1 -= Height * (1f - Pivot.Y);
            constrainRect.X0 += Width * (Pivot.X);
            constrainRect.Y0 += Height * (Pivot.Y);

            Offset = constrainRect.Constrain(Offset);
        }

        /// <summary>
        /// Will OnRender() be allowed to draw outside of this rectangle?
        /// </summary>
        protected ref bool Clipping => ref _clipping;
        public bool Debug {
            get {
#if DEBUG
                return _debug;
#else
                return false;
#endif
            }
            set {
#if DEBUG
                _debug = value;
#endif
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
                _isVisibleNextFrame = value;
            }
        }

        internal ref bool Mounted => ref _mounted;

        /// <summary>
        /// This will be updated whenever everything is rendered, and is calculated from 
        /// RelativeRect and Pivot
        /// </summary>
        protected Rect ScreenRect => _screenRect;

        protected int StackingOffset {
            get => _stackingOffset;
            set => _stackingOffset = value;
        }

        public ArraySlice<Element> Children {
            get => _children;
        }


        public Rect RelativeRect {
            get => _relativeRect;
            set {
                _relativeRect = value;

                if (Parent != null) {
                    Parent.OnChildResize();
                }

                _rectModified = true;
            }
        }

        public float Width => RelativeRect.Width;
        public float Height => RelativeRect.Height;

        public Element this[int index] {
            get {
                return _children[index];
            }
        }

        protected virtual bool SingleChild => false;


        public Element SetChildren(params Element[] newChildren) {
#if DEBUG
            if (SingleChild && _children != null && _children.Count > 1)
                throw new Exception("This element must only be given 1 child, possibly in the constructor.");
#endif

            // child removal is automatically handled, but doing it explicitly here will be O(n) instead of O(n^2)
            for (int i = _children.Count - 1; i >= 0; i--) {
                Remove(i);
            }

            if (newChildren == null)
                return this;

            for (int i = 0; i < newChildren.Length; i++) {
                if (newChildren[i] == null)
                    continue;

                AddChild(newChildren[i]);
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

        private void Add(Element element) {
            _children.Add(element);

            element.Mount();

            _shouldTriggerParentResize = true;
        }

        public void AddChild(Element element) {
            element.Parent = this;
        }

        protected void TriggerLayoutRecalculation() {
            _shouldTriggerParentResize = true;
        }

        public Element Parent {
            get {
                return _parent;
            }
            set {
                if (value == this) {
                    throw new Exception("An element can't be it's own parent, as this causes infinite recursion. Possibly a mistake?");
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



        /// <summary>
        /// Does a linear search up the Element tree and returns an element
        /// if it's type is assignable to T. The search will include the current element.
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
            IsVisible = _isVisibleNextFrame;

            if (!shouldBeVisible) {
                return;
            }

            if (_shouldTriggerParentResize) {
                _shouldTriggerParentResize = false;

                if (Parent != null) {
                    Parent.OnChildResize();
                }
            }

            RecalculateScreenRect(parentScreenRect);
            ResetCoordinates();

            OnUpdate();

            for (int i = 0; i < _children.Count; i++) {
                _children[i].UpdateSelfAndChildren(_screenRect);
            }

            AfterUpdate();
        }


        internal struct RenderAccumulator {
            public int Depth;
            public int StackingDepth;
            public Rect ParentScreenRect;

            public RenderAccumulator(
                int depth,
                int stackingDepth,
                Rect screenRect
            ) {
                StackingDepth = stackingDepth;
                Depth = depth;
                ParentScreenRect = screenRect;
            }
        }

        internal void RenderSelfAndChildren(Rect parentScreenRect) {
            RenderSelfAndChildren(new RenderAccumulator(
                0,
                0,
                parentScreenRect
            ));
        }

        private void RenderSelfAndChildren(RenderAccumulator acc) {
            if (!IsVisible) {
                return;
            }

            int stackingDepth = acc.StackingDepth + _stackingOffset;
            CTX.Current2DDepth = 1f - stackingDepth / 100000f;

            RecalculateScreenRect(acc.ParentScreenRect);
            ResetCoordinates();

            SetTexture(null);

            Rect previousClippingRect = CTX.CurrentClippingRect;
            if (Clipping) {
                CTX.CurrentClippingRect = CTX.CurrentClippingRect.Intersected(_screenRect);
            }

            OnRender();

            for (int i = 0; i < _children.Count; i++) {
                _children[i].RenderSelfAndChildren(new RenderAccumulator(acc.Depth + 1, stackingDepth, _screenRect));
            }


#if DEBUG
            if (Debug) {
                DrawDebugStuff(_screenRect);
            }
#endif

            AfterRender();

            CTX.Current2DDepth = 1 - acc.Depth / 100000f;

            if (Clipping) {
                CTX.CurrentClippingRect = previousClippingRect;
            }
        }

        private void RecalculateScreenRect(Rect parentScreenRect) {
            _screenRect = _relativeRect.Moved(
                (parentScreenRect.X0 + parentScreenRect.Width * _pivot.X) + (_offset.X),
                (parentScreenRect.Y0 + parentScreenRect.Height * _pivot.Y) + (_offset.Y)
            );

            _defaultScreenRect = _screenRect;
        }

        public void ResetCoordinates() {
            CTX.Cartesian2D(1, 1, _screenRect.X0, _screenRect.Y0);
            CTX.SetTransform(Translation(0, 0, CTX.Current2DDepth));
        }


#if DEBUG
        void DrawDebugStuff(Rect newScreenRect) {
            ResetCoordinates();

            Rect prev = CTX.CurrentClippingRect;

            SetDrawColor(1, 1, 1, 0.5f);
            DrawRect(0, 0, Width, Height);

            CTX.CurrentClippingRect = new Rect(0, 0, CTX.ContextWidth, CTX.ContextHeight);

            Color4 col;
            if (ScreenRect != ScreenRect.Rectified()) {
                col = Color4.Red;
            } else {
                col = Color4.Blue;
            }
            col.A = 0.7f;
            SetDrawColor(col);

            DrawRectOutline(1, 0, 0, Width, Height);
            DrawRect(0, 0, 10, 10);
            DrawRect(VW(1) - 10, 0, VW(1), 10);
            DrawRect(0, VH(1) - 10, 10, VH(1));
            DrawRect(VW(1) - 10, VH(1) - 10, VW(1), VH(1));

            SetFont("Consolas", 16);
            DrawText(GetType().Name + ":" + Width + ", " + Height, VW(0.5f), VH(0.5f), HAlign.Center, VAlign.Center);
            DrawText("(" + RelativeRect.X0 + ", " + RelativeRect.Y0 + ")", 0, 0);
            DrawText("(" + RelativeRect.X1 + ", " + RelativeRect.Y1 + ")", Width, Height, HAlign.Right, VAlign.Top);

            Parent.ResetCoordinates();
            float x0 = Pivot.X * Parent.Width;
            float y0 = Pivot.Y * Parent.Height;
            DrawCircle(x0, y0, 5);
            DrawText(GetType().Name + " pivot:" + Pivot.X + ", " + Pivot.Y, x0, y0);

            CTX.CurrentClippingRect = prev;
        }
#endif

        private void Mount() {
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


        internal void Dismount() {
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

            onChildResizeLock = true;

            for (int i = 0; i < _children.Count; i++) {
                if (_children[i]._rectModified)
                    continue;

                _children[i].RelativeRect = _children[i].DefaultRect(Width, Height);
            }

            OnLayout();

            for (int i = 0; i < _children.Count; i++) {
                _children[i]._rectModified = false;
            }

            onChildResizeLock = false;
        }

        /// <summary>
        /// <para>
        /// This is how the parent will size this rect by default. The parent will call this method on it's elements
        /// before calling OnLayout. Most times you don't need this, but there are a few valid uses
        /// </para>
        /// </summary>
        public virtual Rect DefaultRect(float parentWidth, float parentHeight) {
            return Rect.PivotSize(parentWidth, parentHeight, Pivot.X, Pivot.Y);
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


        bool onChildResizeLock = false;
        private void OnChildResize() {
            _shouldTriggerParentResize = false;

            if (onChildResizeLock) {
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

        /// <summary>
        /// Runs after this and all child elements have updated
        /// </summary>
        public virtual void AfterUpdate() {

        }

        /// <summary>
        /// Render cycle that runs every frame.
        /// Frequency is controlled by the window configuration's RenderFrequency.
        /// </summary>
        public virtual void OnRender() {

        }


        /// <summary>
        /// Runs right after all children have been rendered. Mainly used for overlay stuff. Remember to call
        /// ResetCoordinates() here, as the framework doesn't automatically do this due to 
        /// how infrequenty the AfterRender method is needed in practice
        /// </summary>
        public virtual void AfterRender() {

        }



        /// <summary>
        /// Implement cleanup code here. This is ran in children before it is ran in the parents
        /// </summary>
        public virtual void OnDismount() {

        }
    }
}
