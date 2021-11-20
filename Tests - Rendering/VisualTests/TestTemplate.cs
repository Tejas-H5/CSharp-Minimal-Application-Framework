using MinimalAF.Rendering;

namespace MinimalAF.VisualTests
{
	public class TemplateForEntrypointProgram : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Window title goes here";

            CTX.SetClearColor(0, 0, 0, 0);
        }

        public override void OnRender()
        {
            //Rendering code goes here
        }

        public override void OnUpdate()
        {
            //UI Events / input / other non-rendering update code goes here
        }

        public override void OnResize()
        {
        }

        public override void OnCleanup()
        {
        }
    }
}
