﻿using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace MinimalAF {
    public abstract partial class Element {
        static readonly List<Element> NULL_LIST = new List<Element>();

        protected Element _parent = null;
        protected List<Element> _children = NULL_LIST;
        public virtual bool SingleChild => false;

        public bool IsVisibleNextFrame = true;
        protected bool _isVisible = true;
        private bool _shouldTriggerParentResize = false;
        private bool _mounted = false;

        public Vector2 Pivot;
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

        // todo: make this a list
        public List<Element> Children {
            get {
                return _children;
            }
            private set {
#if DEBUG
                if (SingleChild && Children != null && Children.Count > 1)
                    throw new Exception("This element must only be given 1 child, possibly in the constructor.");
#endif

                // Children will be removed anyway, but this should give O(n) instead of O(n^2)
                for (int i = 0; i < Children.Count; i++) {
                    Remove(i);
                }

                _children = value;

                if (Children.Count == 0)
                    return;

                for (int i = 0; i < Children.Count; i++) {
                    Children[i].Parent = this;
                }
                
                _shouldTriggerParentResize = true;
            }
        }

        public Element this[int index] {
            get {
                return Children[index];
            }
        }

        public Element SetChildren(params Element[] arr) {
            Children = new List<Element>(arr);
            return this;
        }

        public void Remove(Element child) {
            int index = Children.IndexOf(child);
            if (index == -1) {
                return;
            }

            Remove(index);
        }

        private void Remove(int index) {
            Element child = Children[index];
            Children.RemoveAt(index);
            child.Dismount();
            child.Parent = null;
        }

        public Element Parent {
            get {
                return _parent;
            }
            set {
                if (value == _parent)
                    return;

                if(_parent != null) {
                    _parent.Remove(this);
                }

                _parent = value;
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

        public bool Clipping { get; set; } = false;

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
                    OnChildResize();
                }
            }

            _screenRect = RelativeRect;
            _screenRect.Move(parentScreenRect.X0, parentScreenRect.Y0);
            CTX.SetScreenRect(_screenRect, false);

            OnUpdate();

            for (int i = 0; i < Children.Count; i++) {
                Children[i].UpdateSelfAndChildren(_screenRect);
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

        public void UseCoordinates() {
            CTX.SetScreenRect(_screenRect, Clipping);
        }

        internal void RenderSelfAndChildren(RenderAccumulator acc) {
            if (!IsVisible) {
                return;
            }

            _screenRect = RelativeRect;
            _screenRect.Move(acc.ParentScreenRect.X0, acc.ParentScreenRect.Y0);
            UseCoordinates();

            SetTexture(null);

            OnRender();

#if DEBUG
            if (MinimalAFEnvironment.Debug) {
                acc = DrawDebugStuff(acc, _screenRect);
            }
#endif

            for (int i = 0; i < Children.Count; i++) {
                Children[i].RenderSelfAndChildren(new RenderAccumulator(acc.Depth + 1, _screenRect, acc.HoverDepth));
            }

            AfterRender();
        }

#if DEBUG
        RenderAccumulator DrawDebugStuff(RenderAccumulator acc, Rect newScreenRect) {
            if (!MouseOverSelf())
                return acc;

            SetDrawColor(Color4.RGBA(1, 0, 0, 0.5f));
            RectOutline(2, 1, 1, VW(1) - 1, VH(1) - 1);

            Rect(0, 0, 10, 10);

            SetDrawColor(Color4.RGB(0, 0, 0));
            int textSize = 12;
            SetFont("Consolas", textSize);
            Text(GetType().Name + " " + RelativeRect.ToString(), -newScreenRect.X0 + 10, -newScreenRect.Y0 + 10 + textSize * acc.HoverDepth);

            SetFont("");
            SetTexture(null);

            acc.HoverDepth += 1;
            return acc;
        }
#endif

        public void Mount() {
            Window w = GetAncestor<Window>();
            if (w == null) {
                return;
            }

            Mount(w);
        }

        private void Mount(Window w) {
            OnMount(w);

            for (int i = 0; i < Children.Count; i++) {
                Children[i].Mount(w);
            }

            _mounted = true;
        }


        public void Dismount() {
            for (int i = 0; i < Children.Count; i++) {
                Children[i].Dismount();
            }

            OnDismount();
            _mounted = false;
        }


        public void Layout() {
            if (!_mounted) {
                return;
            }

            _onChildResizeLock = true;

            for (int i = 0; i < Children.Count; i++) {
                if (Children[i]._rectModified)
                    continue;

                Children[i].RelativeRect = new Rect(0, 0, VW(1), VH(1));
            }

            OnLayout();

            for (int i = 0; i < Children.Count; i++) {
                Children[i]._rectModified = false;
            }

            _onChildResizeLock = false;
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
            for (int i = 0; i < Children.Count; i++) {
                Children[i].Layout();
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
        /// Is called after this element hooks to the element tree.
        /// </para>
        /// </summary>
        public virtual void OnMount(Window w) {

        }

        public virtual void AfterMount() {

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
