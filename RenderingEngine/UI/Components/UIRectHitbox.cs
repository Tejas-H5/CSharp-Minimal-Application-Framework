using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UIRectHitbox : UIHitbox
    {
        private bool _intersectWithParent = false;

        public UIRectHitbox(bool intersectWithParent = false)
        {
            _intersectWithParent = intersectWithParent;
        }

        public override bool PointIsInside(float x, float y)
        {
            return ((!_intersectWithParent) || Intersections.IsInside(x, y, _parent.GetParentRect()))
                && Intersections.IsInside(x, y, _parent.Rect);
        }
    }
}
