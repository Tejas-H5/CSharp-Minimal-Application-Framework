using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI.Core
{
    /// <summary>
    /// Will be used to manage which UIElement is focused in the future
    /// </summary>
    static class UIStateObject
    {
        //Used to draw InverseStencil components to an infinite depth
        private static List<Rect2D> _stencilRectStack = new List<Rect2D>();

        public static Rect2D PeekStencilRect()
        {
            if(_stencilRectStack.Count > 0)
            {
                return _stencilRectStack[_stencilRectStack.Count - 1];
            }

            return Window.Rect;
        }

        public static void PushScreenRectStencil(Rect2D rect)
        {
            Rect2D prevRect = PeekStencilRect();

            var r = prevRect.Intersect(rect);
            _stencilRectStack.Add(r);

            StartInverseStencil(r);
        }

        public static void PopScreenRectStencil()
        {
            if (_stencilRectStack.Count == 0)
                return;

            _stencilRectStack.RemoveAt(_stencilRectStack.Count - 1);
            if (_stencilRectStack.Count == 0)
            {
                CTX.LiftStencil();
                return;
            }

            StartInverseStencil(_stencilRectStack[_stencilRectStack.Count - 1]);
        }

        private static void StartInverseStencil(Rect2D r)
        {
            CTX.StartStencillingWithoutDrawing(inverseStencil: true);
            CTX.DrawRect(r);
            CTX.StartUsingStencil();
        }
    }
}
