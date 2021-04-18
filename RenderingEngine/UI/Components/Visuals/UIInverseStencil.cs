using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.Visuals
{
    public class UIInverseStencil : UIComponent
    {
        public override void BeforeDraw(double deltaTime)
        {
            UIStateObject.PushScreenRectStencil(_parent.Rect);
        }

        public override void AfterDraw(double deltaTime)
        {
            UIStateObject.PopScreenRectStencil();
        }
    }
}
