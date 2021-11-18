using MinimalAF.Datatypes;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF
{
    public partial class Element
    {
		static readonly Element[] NULL_ARRAY = new Element[0];

        public bool IsVisibleNextFrame = true;

        protected RectTransform _rectTransform = new RectTransform();
        protected Element _parent = null;
        protected bool _shouldResize = true;
        protected bool _isVisible = true;

		protected Element[] _children;

		public Element[] Children {
			get {
				return _children;
			}
			set {
				_children = value;
				for (int i = 0; i < _children.Length; i++)
				{
					_children[i].Parent = this;
				}
			}
		}

		public Element Parent {
            get {
                return _parent;
            }
            set {
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

			_children = NULL_ARRAY;
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
			while (next != null)
			{
				if (next is T || tType.IsAssignableFrom(next.GetType()))
					return next as T;

				next = next.Parent;
			}

			return null;
		}

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
		/// Is called after this element hooks to the element tree.
		/// </para>
		/// <para>
		/// Remember to call base.OnStart at some point to initialize the children
		/// </para>
		/// </summary>
		public virtual void OnStart()
		{
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i].Start();
			}
		}


		/// <summary>
		/// The update cycle that runs every frame.
		/// Frequency is controlled by the window configuration's UpdateFrequency
		/// 
		/// If this element is UI, event processing like mouse clicks and keyboard input
		/// should be processed in <see cref="ProcessEvents"/>
		/// </summary>
		public virtual void OnUpdate()
		{
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i].Update();
			}
		}

		/// <summary>
		/// Render cycle that runs every frame.
		/// Frequency is controlled by the window configuration's RenderFrequency.
		/// 
		/// Make sure to call base.OnRender() (NOT Render) to render the children, but AFTER it renders itself
		/// </summary>
		public virtual void OnRender()
		{
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i].Render();
			}
		}

		/// <summary>
		/// Should re-calculate all child rect-transforms if applicable,
		/// and then call OnResize() (And NOT Resize) on them after.
		/// </summary>
		public virtual void OnResize()
		{
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i].Resize();
			}
		}
		public virtual void OnCleanup()
		{
			for (int i = 0; i < _children.Length; i++)
			{
				_children[i].Cleanup();
			}
		}
	}
}
