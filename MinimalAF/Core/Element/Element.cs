using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF {
    public abstract partial class Element {
        static readonly Element[] NULL_ARRAY = new Element[0];

        public virtual bool SingleChild => false;
        public bool IsVisibleNextFrame = true;

        protected Element _parent = null;
        protected bool _isVisible = true;
        protected Element[] _children;
        private bool _shouldTriggerParentResize = false;
        private bool _mounted = false;

        private Rect _relativeRect;
        public Vector2 Pivot;

        // this is auto-calculated
        protected Rect _screenRect;

        public void SetRelativeRect(Rect value) {
            if (RelativeRect != value) {
                RelativeRect = value;
            }

            if (Parent != null) {
                Parent.OnChildResize();
            }
        }


        // TODO: make this somewhat local, or come up with a good way to do that
        protected void SetClearColor(Color4 value) {
            CTX.SetClearColor(value);
        }

        // todo: make this a list
        public Element[] Children {
            get {
                return _children;
            }
            private set {
                if (SingleChild && _children != null && _children.Length > 1)
                    throw new Exception("This element must only be given 1 child, possibly in the constructor.");

                _children = value;

                for (int i = 0; i < _children.Length; i++) {
                    _children[i].Parent = this;
                }
                
                // sepearte loops so that the children have access to all siblings if needed
                for (int i = 0; i < _children.Length; i++) {
                    _children[i].Mount();
                }

                _shouldTriggerParentResize = true;
            }
        }

        public Element SetChildren(params Element[] arr) {
            Children = arr;
            return this;
        }

        public Element Parent {
            get {
                return _parent;
            }
            set {
                if (value == _parent)
                    return;

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

        public bool Clipping { get; set; } = true;

        public Rect RelativeRect {
            get {
                return _relativeRect;
            }
            set {
                _relativeRect = value;

                if (Parent != null) {
                    Parent.OnChildResize();
                }
            }
        }

        public Element() {
            OnConstruct();
        }

        protected virtual void OnConstruct() {
            Children = NULL_ARRAY;
        }

        public Element(Element[] children) {
            Children = children;
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
            CTX.SetScreenRect(_screenRect);

            OnUpdate();

            for (int i = 0; i < _children.Length; i++) {
                _children[i].UpdateSelfAndChildren(_screenRect);
            }

            AfterUpdate();
        }

        internal void RenderSelfAndChildren(Rect parentScreenRect) {
            if (!IsVisible) {
                return;
            }


            _screenRect = RelativeRect;
            _screenRect.Move(parentScreenRect.X0, parentScreenRect.Y0);
            CTX.SetScreenRect(_screenRect);

            CTX.SetScreenRect(_screenRect);
            SetTexture(null);
            SetDrawColor(Color4.RGB(1, 0, 0));
            RectOutline(1, 1, 1, VW(1)-1, VH(1)-1);

            OnRender();

            for (int i = 0; i < _children.Length; i++) {
                _children[i].RenderSelfAndChildren(_screenRect);
            }

            AfterRender();
        }

        public void Mount() {
            Window w = GetAncestor<Window>();
            if(w == null){
                return;
            }

            Mount(w);
        }

        private void Mount(Window w) {
            OnMount(w);

            for(int i = 0; i < Children.Length; i++) {
                Children[i].Mount(w);
            }

            _mounted = true;
        }


        public void Dismount() {
            for (int i = 0; i < _children.Length; i++) {
                _children[i].Dismount();
            }

            OnDismount();
            _mounted = false;
        }


        public void Layout() {
            if(!_mounted) {
                return;
            }

            _onChildResizeLock = true;

            OnLayout();

            _onChildResizeLock = false;
        }

        /// <summary>
        /// Use the newScreenRect to set this element's screenRect. You can optionally make it smaller.
        /// For example, if you are trying to fit to the children inside this element, you will need to 
        /// Call the layout function on each of the chldren. They must then resize to something appropriate, and resize their children
        /// You can use that size to position 
        /// 
        /// This function is required to call Layout on all the children somehow.
        /// Otherwise this element won't work really
        /// </summary>
        /// <param name="newScreenRect"></param>
        public virtual void OnLayout() {
            for (int i = 0; i < _children.Length; i++) {
                _children[i].RelativeRect = new Rect(0, 0, VW(1), VH(1));
            }

            CalculateChildLayouts();
        }

        protected void CalculateChildLayouts() {
            for (int i = 0; i < Children.Length; i++) {
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
