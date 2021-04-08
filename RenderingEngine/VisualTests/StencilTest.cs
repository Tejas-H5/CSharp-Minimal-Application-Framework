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

        float _xPos = 0;

        public override void Render(double deltaTime)
        {
            geometryAndTextTest.Render(deltaTime);
            CTX.Flush();

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.StencilFunc(StencilFunction.Always, 1, ~0);
            GL.StencilMask(~0);

            float size = 60;
            DrawRedRectangle(size, _xPos);

            GL.StencilFunc(StencilFunction.Notequal, 1, ~0);
            GL.StencilMask(0);

            size = 70;
            DrawBlueRectangle(size, _xPos);

            GL.Disable(EnableCap.StencilTest);  
            GL.StencilMask(~0);

        }

        private static void DrawBlueRectangle(float size, float xPos)
        {
            CTX.SetTexture(null);
            CTX.SetDrawColor(0, 0, 1, 1);
            CTX.DrawRect(Window.Width / 2 - size + xPos, Window.Height / 2 - size,
                Window.Width / 2 + size + xPos, Window.Height / 2 + size);
            CTX.Flush();
        }

        private static void DrawRedRectangle(float size, float xPos)
        {
            CTX.SetTexture(null);
            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.DrawRect(Window.Width / 2 - size + xPos, Window.Height / 2 - size,
                Window.Width / 2 + size + xPos, Window.Height / 2 + size);
            CTX.Flush();
        }

        float _time = 0;

        public override void Update(double deltaTime)
        {
            _time += (float)deltaTime;

            geometryAndTextTest.Update(deltaTime);
            _xPos = 200 * MathF.Sin(_time / 2.0f);
        }
    }
}
