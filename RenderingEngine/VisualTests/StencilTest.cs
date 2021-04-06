using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests
{
    //TODO: Get this working at some point, because it is currently not working
    public class StencilTest : EntryPoint
    {
        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Stencil rendering test";

            CTX.SetClearColor(0, 0, 0, 1);
            CTX.SetCurrentFont("Consolas", 24);
        }

        public override void Render(double deltaTime)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

            CTX.Clear();

            GL.StencilFunc(StencilFunction.Always, 1, 0xFF); // all fragments should pass the stencil test
            GL.StencilMask(0xFF); // enable writing to the stencil buffer

            float size = 60;
            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.DrawRect(Window.Width / 2 - size, Window.Height / 2 - size, Window.Width / 2 + size, Window.Height / 2 + size);
            CTX.Flush();

            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF); 
            GL.StencilMask(0x00); // disable writing to the stencil buffer
            GL.Disable(EnableCap.DepthTest);

            size = 70;
            CTX.SetDrawColor(0, 0, 1, 1);
            CTX.DrawRect(Window.Width / 2 - size, Window.Height / 2 - size, Window.Width / 2 + size, Window.Height / 2 + size);
            CTX.Flush();

            GL.StencilMask(0xFF);
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.Enable(EnableCap.DepthTest);

            CTX.Flush();
        }


        public override void Update(double deltaTime)
        {
            
        }
    }
}
