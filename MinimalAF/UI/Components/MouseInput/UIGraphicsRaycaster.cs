using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI.Components.MouseInput
{
    //Sends mouse input to whatever isn't occluded
    public class UIGraphicsRaycaster : UIComponent
    {
        public override bool ProcessEvents()
        {
            bool res = false;

            UIElement root = _parent;

            SendMouseEvents(root);

            return res;
        }

        private void SendMouseEvents(UIElement root)
        {
            _mouseEventArgs.Reset();
            DepthFirstUpdate(root, _parent.Rect, SendMouseEvent);

            _mouseEventArgs.Reset();
            DepthFirstUpdate(root, _parent.Rect, SendMouseWheelEvent);
        }

        MouseEventArgs _mouseEventArgs = new MouseEventArgs();

        private void DepthFirstUpdate(UIElement root, Rect2D occlusionRect, Action<UIElement, MouseEventArgs> intersectAction)
        {
            if (!Intersections.IsInsideRect(Input.MouseX, Input.MouseY, occlusionRect))
                return;

            for(int i = 0; i < root.Count; i++)
            {
                DepthFirstUpdate(root[i], occlusionRect.Intersect(root[i].Rect), intersectAction);
            }

            if (_mouseEventArgs.Handled)
                return;

            //doing a linear search, but the search consists of like 5 things only
            UIHitbox _hitbox = root.GetComponentOfType<UIHitbox>();
            if (_hitbox == null)
                return;

            if (_hitbox.PointIsInside(Input.MouseX, Input.MouseY))
            {
                intersectAction(root, _mouseEventArgs);
            }
        }

        void SendMouseEvent(UIElement root, MouseEventArgs e)
        {
            UIMouseListener mouseFeedback = root.GetComponentOfType<UIMouseListener>();
            mouseFeedback.ProcessMouseButtonEvents(e);
        }

        void SendMouseWheelEvent(UIElement root, MouseEventArgs e)
        {
            UIMouseListener mouseFeedback = root.GetComponentOfType<UIMouseListener>();
            mouseFeedback.ProcessMouseWheelEvents(e);
        }

        public override UIComponent Copy()
        {
            return new UIGraphicsRaycaster();
        }
    }
}
