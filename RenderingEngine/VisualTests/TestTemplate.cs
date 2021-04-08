using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Templates
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
    }
}
