using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering.ImmediateMode
{
    abstract class GeometryDrawer
    {
        //Will be used by child classes to draw their outlines
        protected PolyLineDrawer _outlineDrawer;
        public void SetPolylineDrawer(PolyLineDrawer polyLineDrawer)
        {
            _outlineDrawer = polyLineDrawer;
        }
    }
}
