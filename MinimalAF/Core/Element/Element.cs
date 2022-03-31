using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF {
	public abstract partial class Element {
		static readonly Element[] NULL_ARRAY = new Element[0];

		public virtual bool SingleChild => false;

		public bool IsVisibleNextFrame = true;

		protected Element _parent = null;
		protected bool _shouldMount = false;
		protected bool _isVisible = true;
		protected Element[] _children;

		/// <summary>
        /// Don't use this for rendering, use RelativeRect instead
        /// </summary>a
		internal Rect ScreenRect;
		
		public Rect RelativeRect;

		public Vector2 Pivot;

		// TODO: make this somewhat local, or come up with a good way to do that
		protected void SetClearColor(Color4 value) {
			CTX.SetClearColor(value);
		}

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
				_shouldMount = true;
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

				if (_parent != null) {
					_parent.UpdateLayout();
				}
			}
		}

		public bool Clipping { get; set; } = true;


		public Element() {
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

		public void Update() {
			bool shouldBeVisible = IsVisible;
			IsVisible = IsVisibleNextFrame;

			if (!shouldBeVisible) {
				return;
			}

			if (_shouldMount) {
				_shouldMount = false;
				Mount();
			}

			CTX.SetRect(ScreenRect);
			OnUpdate();

			for (int i = 0; i < _children.Length; i++) {
				_children[i].Update();
			}

			AfterUpdate();
		}

		public void Render() {
			if (!IsVisible) {
				return;
			}

			CTX.SetRect(ScreenRect);
			CTX.Texture.Set(null);

			// SetDrawColor(_clearColor);
			// Rect(RelativeRect);

			OnRender();

			for (int i = 0; i < _children.Length; i++) {
				_children[i].Render();
			}

			AfterRender();
		}

		public void Mount() {
			OnMount();

			AncestorChanged();
			UpdateLayout();
		}

		public void AncestorChanged() {
			OnAncestorChanged();

			for (int i = 0; i < _children.Length; i++) {
				_children[i].AncestorChanged();
			}
		}

		public virtual void OnAncestorChanged() {
		}

		public void Dismount() {
			for (int i = 0; i < _children.Length; i++) {
				_children[i].Dismount();
			}

			OnDismount();
		}

		internal void UpdateLayout() {
			OnLayout();

			for (int i = 0; i < _children.Length; i++) {
				_children[i].UpdateLayout();
			}

			AfterLayout();

			RecalculateScreenRects();
		}

		internal void RecalculateScreenRects() {
			if (Parent == null) {
				// will be overriden by root element
				ScreenRect = GetParentScreenRect();
			} else {
				ScreenRect = RelativeRect;
				ScreenRect.X0 += Parent.ScreenRect.X0;
				ScreenRect.Y0 += Parent.ScreenRect.Y0;
			}

			for (int i = 0; i < _children.Length; i++) {
				_children[i].RecalculateScreenRects();
			}
		}


		/// <summary>
		/// <para>
		/// Is called after this element hooks to the element tree.
		/// </para>
		/// </summary>
		public virtual void OnMount() {

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
        /// Use this to re-calculate all of the child RelativeRects. They are Rects that are defined relative to this one.
        /// Don't change this container's own rectangle, as you may be overriding changes done by parents
        /// 
        /// Note that in most cases you can write less code by using the Layout methods rather than 
        /// setting the rects by hand
		/// </summary>
		public virtual void OnLayout() {
		}


		/// <summary>
		/// Use this to arrange all children containers, and fix up this element's size
		/// so that parent elements can arrange this
		/// </summary>
		public virtual void AfterLayout() {

		}

		/// <summary>
		/// Implement cleanup code here. This is ran in children before it is ran in the parents
		/// </summary>
		public virtual void OnDismount() {

		}
	}
}
