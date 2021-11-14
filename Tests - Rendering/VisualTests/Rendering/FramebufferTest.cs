using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF.Util;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
    public class FramebufferTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "FramebufferTest";

            //w.RenderFrequency = 120;
            //w.UpdateFrequency = 120; 20;

            CTX.SetClearColor(1, 1, 1, 1);
        }


        double timer = 0;

        public override void OnUpdate()
        {
            timer += Time.DeltaTime;
        }

        public override void OnRender()
        {
            CTX.UseFramebufferTransparent(0);

            CTX.SetDrawColor(0, 0, 1, 1);

            float wCX = Rect.CenterX;
            float wCY = Rect.CenterY;
            DrawDualCirclesCenter(wCX, wCY);
            CTX.SetDrawColor(1, 1, 0, 1);
            CTX.DrawRect(wCX, wCY, wCX + 50, wCY + 25);

            CTX.StopUsingFramebuffer();

            CTX.SetDrawColor(new Color4(0, 0, 0, 1));
            CTX.SetCurrentFont("Consolas", 12);
            CTX.DrawText("The red square must be fully visible under the circles.\n" +
                "The part where the circles overlap must not be visible.\n" +
                "There must be a small orange rectangle in the middle\n" +
                "This text must be 0,0,0 black \n",
                0, Height - 20);

            CTX.SetDrawColor(1, 0, 0, 1);

            float rectSize = 200;


            CTX.DrawRect(wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);
            CTX.SetDrawColor(1, 1, 1, 0.5f);
            CTX.SetTextureToFramebuffer(0);
            
            CTX.DrawRect(Left, Bottom, Width,Height);

            CTX.SetTexture(null);

            CTX.SetDrawColor(0, 1, 0, 0.5f);
            CTX.DrawRectOutline(10, wCX - 300, wCY - 300, wCX + 300, wCY + 300);
        }


        private static void DrawDualCirclesCenter(float x, float y)
        {
            CTX.DrawCircle(x - 100, y - 100, 200);
            CTX.DrawCircle(x + 100, y + 100, 200);
        }
    }
}
