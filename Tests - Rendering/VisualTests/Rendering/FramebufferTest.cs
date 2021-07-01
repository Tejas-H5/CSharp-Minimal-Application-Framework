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
        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "FramebufferTest";

            //Window.RenderFrequency = 120;
            //Window.UpdateFrequency = 20;

            CTX.SetClearColor(1, 1, 1, 1);
        }


        double timer = 0;

        public override void Update(double deltaTime)
        {
            timer += deltaTime;
        }

        public override void Render(double deltaTime)
        {
            CTX.UseFramebufferTransparent(0);

            CTX.SetDrawColor(0, 0, 1, 1);

            float wCX = Window.Width / 2;
            float wCY = Window.Height / 2;
            DrawDualCirclesCenter(wCX, wCY);
            CTX.SetDrawColor(1, 1, 0, 1);
            CTX.DrawRect(wCX, wCY, wCX + 50, wCY + 25);

            CTX.StopUsingFramebuffer();

            CTX.SetDrawColor(new Color4(0, 0, 0, 1));
            CTX.SetCurrentFont("Consolas", 12);
            CTX.DrawText("The red square must be fully visible under the circles.\n" +
                "The part where the circles overlap must not be visible.\n" +
                "This text must be 0,0,0 black \n",
                0, Window.Height - 20);

            CTX.SetDrawColor(1, 0, 0, 1);

            float rectSize = 200;


            CTX.DrawRect(wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);
            CTX.SetDrawColor(1, 1, 1, 0.5f);
            CTX.SetTextureToFramebuffer(0);
            
            CTX.DrawRect(0, 0, Window.Width,Window.Height);

            CTX.SetTexture(null);

            CTX.SetDrawColor(0, 1, 0, 0.5f);
            CTX.DrawRectOutline(10, wCX - 300, wCY - 300, wCX + 300, wCY + 300);
        }


        private static void DrawDualCirclesCenter(float x, float y)
        {
            CTX.DrawCircle(x - 100, y - 100, 200);
            CTX.DrawCircle(x + 100, y + 100, 200);

            CTX.DrawRect(x, y, x + 10, x + 10);
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
