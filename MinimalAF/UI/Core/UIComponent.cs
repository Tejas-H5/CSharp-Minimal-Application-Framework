using System;

namespace MinimalAF.UI.Core
{
    public abstract class UIComponent
    {
        protected UIElement _parent;
        public UIElement Parent { get { return _parent; } }

        public virtual void SetParent(UIElement parent)
        {
            _parent = parent;
        }

        public virtual void Update(double deltaTime) { }

        public virtual void BeforeDraw(double deltaTime) { }
        public virtual void Draw(double deltaTime) { }
        public virtual void AfterDraw(double deltaTime) { }
        public virtual bool ProcessEvents() { return false; }
        public virtual void OnResize() { }

        public abstract UIComponent Copy();
    }
}
