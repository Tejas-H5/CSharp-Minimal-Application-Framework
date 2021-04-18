using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.Visuals
{
    //TODO: The problem with the current implementation is that
    //only UIElements that don't have any parents that use this component can use this component
    //without overwriting a parent's stencil buffer that they were using.
    //The implementation of the stencilling will have to change to be useing steps, so
    //each level deep is only incrementing all existing stencil buffer values before writing zeroes,
    //and then decrementing on the way out.
    //seems a bit of effort to implement properly so I might do it when I actually need it
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
