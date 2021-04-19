namespace RenderingEngine.UI.Core
{
    public abstract class UIComponent
    {
        protected UIElement _parent;
        public UIElement Parent { get { return _parent; } }

        public virtual void SetParent(UIElement parent)
        {
            if (_parent != null)
                _parent.RectTransform.OnDataChanged -= OnRectTransformResize;

            _parent = parent;

            if (_parent != null)
                _parent.RectTransform.OnDataChanged += OnRectTransformResize;
        }

        public virtual void Update(double deltaTime) { }

        public virtual void BeforeDraw(double deltaTime) { }
        public virtual void Draw(double deltaTime) { }
        public virtual void AfterDraw(double deltaTime) { }
        public virtual bool ProcessEvents() { return false; }

        public virtual void OnResize() {
            OnRectTransformResize(_parent.RectTransform);
        }

        protected virtual void OnRectTransformResize(UIRectTransform rtf) { }
    }
}
