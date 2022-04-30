using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace MinimalAF {
    public abstract partial class Element {
        protected Element parent = null;
        protected List<Element> children = new List<Element>();

        internal static List<Element> RenderQueue = new List<Element>();
        protected int stackingOffset = 0;

        public virtual bool SingleChild => false;

        public bool IsVisibleNextFrame = true;
        protected bool isVisible = true;
        private bool shouldTriggerParentResize = false;

        // IDK a better name for this
        public void TriggerLayoutRecalculation() {
            shouldTriggerParentResize = true;
        }

        private bool mounted = false;
        internal bool Mounted {
            set {
                mounted = value;
            }
        }

        public Vector2 Pivot;
        public Vector2 Offset;
        private Rect relativeRect;
        internal Rect screenRect; // this is auto-calculated
        internal Rect defaultScreenRect; // this is auto-calculated

        /// <summary>
        /// This will be updated whenever everything is rendered
        /// </summary>
        public Rect ScreenRect => screenRect;

        bool rectModified = false;

        public bool Clipping {
            get; set;
        }


        // TODO: make this somewhat local, or come up with a good way to do that
        protected void SetClearColor(Color4 value) {
            CTX.SetClearColor(value);
        }

        public ArraySlice<Element> Children {
            get {
                return children;
            }
        }


        public Element this[int index] {
            get {
                return children[index];
            }
        }

        public Element SetChildren(params Element[] newChildren) {
#if DEBUG
            if (SingleChild && children != null && children.Count > 1)
                throw new Exception("This element must only be given 1 child, possibly in the constructor.");
#endif

            // children will be removed anyway, but this should give O(n) instead of O(n^2)
            for (int i = children.Count - 1; i >= 0; i--) {
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
            int index = children.IndexOf(child);
            if (index == -1) {
                return;
            }

            Remove(index);
        }

        private void Remove(int index) {
            Element child = children[index];
            children.RemoveAt(index);
            child.Dismount();
            child.parent = null;

            shouldTriggerParentResize = true;
        }

        public int Index() {
            if (Parent == null) {
                return 0;
            }

            return Parent.children.IndexOf(this);
        }

        private void Add(Element element) {
            children.Add(element);

            element.Mount();

            shouldTriggerParentResize = true;
        }

        public void AddChild(Element element) {
            element.Parent = this;
        }

        public Element Parent {
            get {
                return parent;
            }
            set {
                if (value == this) {
                    throw new Exception("DevIsStupid exception: An element can't be it's own parent, as this causes infinite recursion");
                }

                if (value == parent)
                    return;

                if (parent != null) {
                    parent.RemoveChild(this);
                }

                parent = value;

                if (parent != null) {
                    parent.Add(this);
                }
            }
        }

        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                if (isVisible == value)
                    return;

                isVisible = value;
                IsVisibleNextFrame = value;
            }
        }

        public Rect RelativeRect {
            get {
                return relativeRect;
            }
            set {
                relativeRect = value;

                if (Parent != null) {
                    Parent.OnChildResize();
                }

                rectModified = true;
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

            if (shouldTriggerParentResize) {
                shouldTriggerParentResize = false;

                if (Parent != null) {
                    Parent.OnChildResize();
                }
            }

            RecalcScreenRect(parentScreenRect);
            ResetCoordinates();

            OnUpdate();

            for (int i = 0; i < children.Count; i++) {
                children[i].UpdateSelfAndChildren(screenRect);
            }

            AfterUpdate();
        }


        internal struct RenderAccumulator {
            public int Depth;
            public int StackingDepth;
            public Rect ParentScreenRect;

#if DEBUG
            public int HoverDepth;
#endif

            public RenderAccumulator(
                int depth,
                int stackingDepth,
                Rect screenRect
#if DEBUG
                , int hoverDepth
#endif
            ) {
                StackingDepth = stackingDepth;
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
                0,
                parentScreenRect
#if DEBUG
                , 0
#endif
            ));
        }

        private void RenderSelfAndChildren(RenderAccumulator acc) {
            if (!IsVisible) {
                return;
            }

            int stackingDepth = acc.StackingDepth + stackingOffset;
            // A hack that allows children to render above parents if needed.
            // I am still not quite sure why it works.
            // Side-effect: UIs can only be 100,000 elements deep
            CTX.Current2DDepth = 1f -stackingDepth / 100000f;

            RecalcScreenRect(acc.ParentScreenRect);
            ResetCoordinates();

            SetTexture(null);

            Rect previousClippingRect = CTX.CurrentClippingRect;
            if (Clipping) {
                CTX.CurrentClippingRect = CTX.CurrentClippingRect.Intersect(screenRect);
            }

            OnRender();

#if DEBUG
            if (MinimalAFEnvironment.Debug) {
                acc = DrawDebugStuff(acc, screenRect);
            }
#endif

            for (int i = 0; i < children.Count; i++) {
                children[i].RenderSelfAndChildren(new RenderAccumulator(acc.Depth + 1, stackingDepth, screenRect
#if DEBUG
                    , acc.HoverDepth
#endif           
                ));
            }

            AfterRender();

            CTX.Current2DDepth = 1 - acc.Depth / 100000f;

            if (Clipping) {
                CTX.CurrentClippingRect = previousClippingRect;
            }
        }

        private void RecalcScreenRect(Rect parentScreenRect) {
            screenRect = RelativeRect;
            screenRect.Move(parentScreenRect.X0 + Offset.X, parentScreenRect.Y0 + Offset.Y);

            defaultScreenRect = screenRect;
        }

        public void ResetCoordinates() {
            CTX.SetScreenRect(screenRect);
            CTX.SetTransform(Translation(0, 0, CTX.Current2DDepth));
        }


#if DEBUG
        RenderAccumulator DrawDebugStuff(RenderAccumulator acc, Rect newScreenRect) {
            if (!MouseOverSelf)
                return acc;

            ResetCoordinates();

            SetDrawColor(Color4.RGBA(1, 0, 0, 0.5f));

            DrawRectOutline(2, 1, 1, VW(1) - 1, VH(1) - 1);

            SetDrawColor(Color4.RGBA(1, 0, 0, 0.35f));
            DrawRect(0, 0, 10, 10);
            DrawRect(VW(1) - 10, 0, VW(1), 10);
            DrawRect(0, VH(1) - 10, 10, VH(1));
            DrawRect(VW(1) - 10, VH(1) - 10, VW(1), VH(1));

            SetDrawColor(Color4.RGB(0, 0, 0));
            int textSize = 12;

            string fontName = CTX.Text.ActiveFont.FontName;
            int size = CTX.Text.ActiveFont.FontSize;
            SetFont("Consolas", textSize);

            string text = GetType().Name + " " + RelativeRect.ToString() + " Depth: " + acc.Depth;
            DrawText(text, -newScreenRect.X0 + 10, -newScreenRect.Y0 + 10 + textSize * acc.HoverDepth);

            SetFont(fontName, size);

            acc.HoverDepth += 1;
            return acc;
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

            for (int i = 0; i < children.Count; i++) {
                children[i].Mount(w);
            }

            AfterMount(w);

            mounted = true;
        }


        internal void Dismount() {
            for (int i = 0; i < children.Count; i++) {
                children[i].Dismount();
            }

            OnDismount();
            mounted = false;
        }


        internal void Layout() {
            if (!mounted) {
                return;
            }

            onChildResizeLock = true;

            for (int i = 0; i < children.Count; i++) {
                if (children[i].rectModified)
                    continue;

                children[i].RelativeRect = DefaultRect();
            }

            OnLayout();

            for (int i = 0; i < children.Count; i++) {
                children[i].rectModified = false;
            }

            onChildResizeLock = false;
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
            for (int i = 0; i < children.Count; i++) {
                children[i].Layout();
            }
        }


        bool onChildResizeLock = false;
        private void OnChildResize() {
            shouldTriggerParentResize = false;

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


        public virtual void AfterRender() {

        }



        /// <summary>
        /// Implement cleanup code here. This is ran in children before it is ran in the parents
        /// </summary>
        public virtual void OnDismount() {

        }
    }
}
