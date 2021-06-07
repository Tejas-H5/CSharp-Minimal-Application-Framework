using MinimalAF.Rendering;
using MinimalAF.UI;

namespace MinimalAF.UI
{
    /// <summary>
    /// Used to prevent drawing outside of a UI element's rectangle, like a stencil.
    /// 
    /// Then why is it called 'inverse stencil'? 
    /// because it sounds cool
    /// </summary>
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

        public override UIComponent Copy()
        {
            return new UIInverseStencil();
        }
    }
}
