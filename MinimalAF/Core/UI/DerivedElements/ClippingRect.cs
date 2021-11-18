using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;

namespace MinimalAF
{
    /// <summary>
    /// Used to prevent drawing outside of a UI element's rectangle, like a stencil.
    /// 
    /// Then why is it called 'inverse stencil'? 
    /// because it sounds cool
    /// </summary>
    public class ClippingRect : Element
    {
        ClippingRectState _state;

        public override void OnStart()
        {
            _state = GetAncestor<ClippingRectState>();
        }

        public override void OnRender()
        {
            PushStencil();

            _state.Render();

            PopStencil();
        }

        private void PushStencil()
        {
            _state.PushRect(_parent.Rect);

            StartInverseStencil(_state.PeekRect());
        }

        private void PopStencil()
        {
            _state.PopRect();

            if (_state.Count == 0)
            {
                CTX.LiftStencil();
            }
            else
            {
                StartInverseStencil(_state.PeekRect());
            }
        }

        private static void StartInverseStencil(Rect2D r)
        {
            CTX.StartStencillingWithoutDrawing(inverseStencil: true);
            CTX.DrawRect(r);
            CTX.StartUsingStencil();
        }
    }
}
