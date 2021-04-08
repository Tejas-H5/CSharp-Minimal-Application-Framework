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

            geometryAndTextTest.Init();
        }

        GeometryAndTextTest geometryAndTextTest = new GeometryAndTextTest();

        public override void Render(double deltaTime)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);


            GL.StencilMask(0x00);

            //geometryAndTextTest.Render(deltaTime);

            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);

            float size = 60;
            float xPos = -100;
            DrawRedRectangle(size, xPos);

            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilMask(0x00);
            GL.Disable(EnableCap.DepthTest);

            size = 70;
            DrawBlueRectangle(size, xPos);
            CTX.Flush();

            //GL.StencilMask(0xFF);
            //GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            //GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);


        }

        private static void DrawBlueRectangle(float size, float xPos)
        {
            CTX.SetTexture(null);
            CTX.SetDrawColor(0, 0, 1, 1);
            CTX.DrawRect(Window.Width / 2 - size + xPos, Window.Height / 2 - size,
                Window.Width / 2 + size + xPos, Window.Height / 2 + size);
        }

        private static void DrawRedRectangle(float size, float xPos)
        {
            CTX.SetTexture(null);
            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.DrawRect(Window.Width / 2 - size + xPos, Window.Height / 2 - size,
                Window.Width / 2 + size + xPos, Window.Height / 2 + size);
        }

        public override void Update(double deltaTime)
        {
            geometryAndTextTest.Update(deltaTime);
        }
    }
}
