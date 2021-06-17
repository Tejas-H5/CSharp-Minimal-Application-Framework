using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.Util;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
    public class FramebufferTest : EntryPoint
    {
        Framebuffer _fb;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "FramebufferTest";

            //Window.RenderFrequency = 120;
            //Window.UpdateFrequency = 20;

            CTX.SetClearColor(1, 1, 1, 1);

            _fb = new Framebuffer(1,1,new TextureImportSettings());
            _fb.Resize(600, 600);
        }


        double timer = 0;

        public override void Update(double deltaTime)
        {
            timer += deltaTime;
        }

        public override void Render(double deltaTime)
        {
            _fb.Resize(Window.Width, Window.Height);

            CTX.SetDrawColor(new Color4(0, 0, 0, 1));
            CTX.SetCurrentFont("Consolas", 12);
            CTX.DrawText("The red square must be fully visible under the circles.\n" +
                "The part where the circles overlap must not be visible.",
                0, Window.Height - 20);

            CTX.SetDrawColor(1, 0, 0, 1);

            float rectSize = 200;

            float wCX = Window.Width / 2;
            float wCY = Window.Height / 2;

            CTX.DrawRect(wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);
            CTX.Flush();

            CTX.SetDrawColor(1, 1, 1, 0.5f);
            CTX.SetTexture(_fb.Texture);
            //CTX.DrawRect(wCX - 300, wCY + 300, wCX + 300, wCY - 300);
            CTX.DrawRect(0, 0, Window.Width,Window.Height);
            CTX.SetTexture(null);

            CTX.SetDrawColor(0, 1, 0, 0.5f);
            CTX.DrawRectOutline(10, wCX - 300, wCY - 300, wCX + 300, wCY + 300);

            CTX.Flush();
            _fb.Use();

            CTX.SetDrawColor(0, 0, 1, 1);
            DrawDualCirclesCenter();

            CTX.Flush();
            _fb.StopUsing();

            GL.Viewport(0, 0, Window.Width, Window.Height);
            CTX.Viewport2D(Window.Width, Window.Height);
        }


        private static void DrawDualCirclesCenter()
        {
            float wCX = Window.Width / 2;
            float wCY = Window.Height / 2;

            CTX.DrawCircle(wCX - 100, wCY - 100, 200);
            CTX.DrawCircle(wCX + 100, wCY + 100, 200);
        }

        private static void DrawDualCircles()
        {
            CTX.DrawCircle(300 - 100, 300 - 100, 200);
            CTX.DrawCircle(300 + 100, 300 + 100, 200);
        }

        public override void Resize()
        {
            base.Resize();
        }
    }
}
