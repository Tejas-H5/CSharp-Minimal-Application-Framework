using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UIRectHitbox : UIHitbox
    {
        public override bool PointIsInside(float x, float y)
        {
            return Intersections.IsInside(x, y, _parent.Rect);
        }
    }
}
