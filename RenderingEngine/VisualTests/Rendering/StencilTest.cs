using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using System;

namespace RenderingEngine.VisualTests.Rendering
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
            CTX.StartStencillingWithoutDrawing(true);

            float barSize = MathF.Abs((Window.Height / 2 - 5) * MathF.Sin(_time / 4f));
            CTX.DrawRect(0, Window.Height, Window.Width, Window.Height - barSize);
            CTX.DrawRect(0, 0, Window.Width, barSize);

            CTX.StartUsingStencil();

            geometryAndTextTest.Render(deltaTime);

            CTX.LiftStencil();

            CTX.StartStencillingWhileDrawing();

            float size = 60;
            DrawRedRectangle(size, _xPos);

            CTX.StartUsingStencil();

            size = 70;
            DrawBlueRectangle(size, _xPos);

            CTX.LiftStencil();
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
