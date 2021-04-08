using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI
{
    public abstract class UIComponent
    {
        protected UIElement _parent;

        public virtual void SetParent(UIElement parent)
        {
            _parent = parent;
        }

        public virtual void Update(double deltaTime) { }
        public virtual void Draw(double deltaTime) { }
        public virtual bool ProcessEvents() { return false; }
    }
}
