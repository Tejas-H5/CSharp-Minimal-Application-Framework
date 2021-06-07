using MinimalAF.UI;

namespace MinimalAF.UI
{
    public abstract class UIHitbox : UIComponent
    {
        public bool PointIsInside(float x, float y)
        {
            return PointIsInsideInternal(x, y);
        }

        protected abstract bool PointIsInsideInternal(float x, float y);
    }
}
