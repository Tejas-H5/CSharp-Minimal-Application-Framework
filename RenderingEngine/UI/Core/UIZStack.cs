using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Core
{
    public class UIZStack : UIElement
    {
        public UIZStack()
        {
        }

        public override void Draw(double deltaTime)
        {
            DrawIfVisible(deltaTime);
        }

        public override void DrawIfVisible(double deltaTime)
        {
            if (!_isVisible)
                return;

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].DrawIfVisible(deltaTime);
            }
        }

        public override void UpdateChildren(double deltaTime)
        {
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                if (_children[i].IsVisible)
                {
                    _children[i].Update(deltaTime);
                }
            }
        }

        internal override bool ProcessChildEvents()
        {
            bool res = false;
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                if (_children[i].IsVisible)
                {
                    res = _children[i].ProcessChildEvents();
                    break;
                }
            }

            if (res)
                return true;

            return ProcessComponentEvents();
        }
    }
}
