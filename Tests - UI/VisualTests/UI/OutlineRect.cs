using MinimalAF.Datatypes;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.UI
{
    public class OutlineRect : Container
    {
        Color4 _col;
        float _thickness;
        public OutlineRect(Color4 col, float thickness, params Element[] children) : base(children)
        {
            _thickness = thickness;
            _col = col;
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(_col);

            CTX.DrawRectOutline(_thickness, Rect);

            base.OnRender();
        }
    }
}
