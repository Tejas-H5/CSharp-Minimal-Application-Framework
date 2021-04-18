namespace RenderingEngine.UI.Components.MouseInput
{
    public class UIRectHitbox : UIHitbox
    {
        private bool _intersectWithParent;

        public UIRectHitbox(bool intersectWithParent = true)
        {
            _intersectWithParent = intersectWithParent;
        }

        public override bool PointIsInside(float x, float y)
        {
            return (!_intersectWithParent || Intersections.IsInside(x, y, _parent.GetParentRect()))
                && Intersections.IsInside(x, y, _parent.Rect);
        }
    }
}
