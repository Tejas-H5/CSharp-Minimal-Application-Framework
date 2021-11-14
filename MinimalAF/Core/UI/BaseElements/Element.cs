using MinimalAF.Datatypes;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF
{
    public abstract class Element
    {
        public bool IsVisibleNextFrame = true;

        protected RectTransform _rectTransform = new RectTransform();
        protected Element _parent = null;
        protected bool _shouldResize = true;
        protected bool _isVisible = true;


        public RectTransform RectTransform { get { return _rectTransform; } }

        public Rect2D Rect {
            get { return _rectTransform.Rect; }
            set {
                _rectTransform.Rect = value;
            }
        }

        public virtual Rect2D GetParentRect()
        {
            if (Parent != null)
            {
                return Parent.Rect;
            }

            throw new NotImplementedException("Parent wasn't hooked up properly. " +
                "Or you called GetParentRect on a 'Window' element. which shouldn't have any parents");
        }

        public float Width { get { return Rect.Width; } }
        public float Height { get { return Rect.Height; } }
        public float Left { get { return Rect.Left; } }
        public float Bottom { get { return Rect.Bottom; } }
        public float Right { get { return Rect.Right; } }
        public float Top { get { return Rect.Top; } }


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

        public Element Parent {
            get {
                return _parent;
            }
            set {
                // If required: hooking and unhooking should happen here

                _parent = value;
                _shouldResize = true;
            }
        }

        public bool IsVisible {
            get { return _isVisible; }
            set {
                _isVisible = value;
                IsVisibleNextFrame = value;

                if (_parent != null)
                {
                    _parent.Resize();
                }
            }
        }

        public Element()
        {
            RectTransform.Anchors(new Rect2D(0, 0, 1, 1));
            RectTransform.Offsets(new Rect2D(0, 0, 0, 0));
        }

        public virtual void OnStart() { }

        public virtual void OnCleanup() { }

        /// <summary>
        /// The update cycle that runs every frame.
        /// Frequency is controlled by the window configuration's UpdateFrequency
        /// 
        /// If this element is UI, event processing like mouse clicks and keyboard input
        /// should be processed in <see cref="ProcessEvents"/>
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Render cycle that runs every frame.
        /// Frequency is controlled by the window configuration's RenderFrequency.
        /// 
        /// Make sure to call base.OnRender() (NOT Render) to render the children, but AFTER it renders itself
        /// </summary>
        public virtual void OnRender() { }

        /// <summary>
        /// Should re-calculate all child rect-transforms if applicable,
        /// and then call Resize() (And NOT OnResize) on them after.
        /// </summary>
        public virtual void OnResize() { }

        /// <summary>
        /// <para>
        /// If this Element is UI, process events here rather than 
        /// in <see cref="OnUpdate"/>, so that if we have 2 things on top of each other for example, only the deeper child
        /// will be interacted with.
        /// Assumes that the mouse can't be in two places at once.
        /// </para>
        /// <para>
        /// (Don't do this if you need to respond to a keypress from anywhere in the program)
        /// </para>
        /// <para>
        /// This function should get every 'active' child to process their events, and if none have been processed yet,
        /// process our own. 
        /// </para>
        /// <para>
        /// What is considered 'active' may be decided by the implementation. For instance, a normal UI will
        /// consider anything that isn't visible as active. A UI container stacking UI on top of each other however,
        /// may decide that only the top-most UI is active despite all of them being rendered.
        /// </para>
        /// It is an object-recursive function called before <see cref="Update"/>. The recursion is
        /// started by any <see cref="Element"/> that has <see cref="Parent"/> set to null.
        /// 
        /// </summary>
        /// <returns>Return 'true' if we were able to process an event, false otherwise.</returns>
        public virtual bool ProcessEvents() { return false; }


        public void Update()
        {
            bool shouldBeVisible = IsVisible;
            IsVisible = IsVisibleNextFrame;

            if (!shouldBeVisible)
            {
                return;
            }

            if (_shouldResize)
            {
                _shouldResize = false;
                Resize();
            }

            if (_parent == null)
            {
                ProcessEvents();
            }

            OnUpdate();
        }

        public void Render()
        {
            if (!IsVisible)
            {
                return;
            }

            OnRender();
        }

        public void Start()
        {
            OnStart();
        }

        public void Cleanup()
        {
            OnCleanup();
        }

        public void UpdateRect()
        {
            _rectTransform.UpdateRectFromOffset(GetParentRect());
        }

        internal void Resize()
        {
            UpdateRect();

            OnResize();
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
        public T GetAncestor<T>() where T : Element
        {
            Type tType = typeof(T);
            Element next = this;
            while(next != null)
            {
                if (next is T || tType.IsAssignableFrom(next.GetType()))
                    return next as T;

                next = next.Parent;
            }

            return null;
        }
    }
}
