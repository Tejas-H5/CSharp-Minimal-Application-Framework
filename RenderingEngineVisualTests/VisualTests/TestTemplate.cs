using RenderingEngine.Logic;
using RenderingEngine.Rendering;

namespace RenderingEngine.VisualTests
{
    public class TemplateForEntrypointProgram : EntryPoint
    {
        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Window title goes here";

            CTX.SetClearColor(0, 0, 0, 0);
        }

        public override void Render(double deltaTime)
        {
            //Rendering code goes here
        }

        public override void Update(double deltaTime)
        {
            //UI Events / input / other non-rendering update code goes here
        }

        public override void Resize()
        {
            //Don't delete base.Resize()
            base.Resize();
        }

        public override void Cleanup()
        {
            //Don't delete base.Cleanup()
            base.Cleanup();
        }
    }
}
