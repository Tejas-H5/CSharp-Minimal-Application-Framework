using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components
{
    public abstract class UIHitbox : UIComponent
    {
        public abstract bool PointIsInside(float x, float y);
    }
}
