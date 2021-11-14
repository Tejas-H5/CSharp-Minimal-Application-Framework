using MinimalAF.Datatypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
    public class ClippingRectState : Container
    {
        //Used to draw InverseStencil components to an infinite depth
        private static List<Rect2D> _stencilRectStack = new List<Rect2D>();

        public ClippingRectState(params Element[] children) : base(children)
        {
        }


        public int Count {
            get {
                return _stencilRectStack.Count;
            }
        }

        public Rect2D PeekRect()
        {
            if (_stencilRectStack.Count > 0)
            {
                return _stencilRectStack[_stencilRectStack.Count - 1];
            }
            return GetAncestor<Window>().Rect;
        }

        public void PushRect(Rect2D rect)
        {
            Rect2D prevRect = PeekRect();
            var r = prevRect.Intersect(rect);
            _stencilRectStack.Add(r);
        }

        public void PopRect()
        {
            if (_stencilRectStack.Count == 0)
                return;
            _stencilRectStack.RemoveAt(_stencilRectStack.Count - 1);
        }
    }
}