using MinimalAF.UI;

namespace MinimalAF.UI
{
    public class UIRectHitbox : UIHitbox
    {
        private bool _intersectWithParent;

        public UIRectHitbox(bool intersectWithParent = true)
        {
            _intersectWithParent = intersectWithParent;
        }

        public override UIComponent Copy()
        {
            return new UIRectHitbox(_intersectWithParent);
        }

        protected override bool PointIsInsideInternal(float x, float y)
        {
            return (!_intersectWithParent || Intersections.IsInsideRect(x, y, _parent.GetParentRect()))
                && Intersections.IsInsideRect(x, y, _parent.Rect);
        }
    }
}
