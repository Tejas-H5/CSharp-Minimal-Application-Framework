using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF {
	public partial class Element {
		static readonly Element[] NULL_ARRAY = new Element[0];

		public virtual bool SingleChild => false;

		public bool IsVisibleNextFrame = true;

		protected Element _parent = null;
		protected bool _shouldResize = true;
		protected bool _isVisible = true;
		protected Element[] _children;
		private Color4 _clearColor;

		public Rect ScreenRect;
		public Vector2 Pivot;

		protected void SetClearColor(Color4 value) {
			_clearColor = value;
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

		public void SetChildren(params Element[] arr) {
			Children = arr;
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
			get {
				return _isVisible;
			}
			set {
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

			if (_shouldResize) {
				_shouldResize = false;
				UpdateLayout();
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
			CTX.SetClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
			CTX.Texture.Set(null);

			OnRender();

			for (int i = 0; i < _children.Length; i++) {
				_children[i].Render();
			}

			AfterRender();
		}


		// Rename to mount and OnMount
		public void Mount() {
			OnMount();

			for (int i = 0; i < _children.Length; i++) {
				_children[i].Mount();
			}

			AfterMount();

			UpdateLayout();
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
		/// Recalculate an element's layout here using the parent's layout.
		/// </summary>
		public virtual void OnLayout() {

		}


		/// <summary>
		/// Recalculate an element's layout here based on the child's layout. This is how
		/// you would implement auto-resizing to a child's content for example
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
