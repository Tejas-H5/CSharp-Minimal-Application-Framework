using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI
{
    public class EventState
    {
        public bool Handled = false;
    }

    public class UINode : UIElement
    {
        protected List<UIElement> _children = new List<UIElement>();

        public void AddChild(UIElement element)
        {
            _children.Add(element);
            element.Parent = this;
        }

        public override void Update(double deltaTime)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Update(deltaTime);
            }

            if (_parent == null)
            {
                ProcessChildEvents();
            }
        }

        internal override bool ProcessChildEvents()
        {
            bool hasProcessed = false;
            for (int i = 0; i < _children.Count; i++)
            {
                hasProcessed = hasProcessed || _children[i].ProcessChildEvents();
            }

            if (hasProcessed)
                return true;

            return ProcessEvents();
        }


        public override void Draw(double deltaTime)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Draw(deltaTime);
            }
        }

        public override void Resize()
        {
            base.Resize();
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Resize();
            }
        }
    }
}
