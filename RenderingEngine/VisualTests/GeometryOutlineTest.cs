using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Rendering;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class GeometryOutlineTest : EntryPoint
    {
        public override void Start()
        {
			
            Window.Size = (800, 600);
            Window.Title = "Triangle";
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1,1,1);
        }


        public override void Update(double deltaTime)
        {

        }

        public override void Render(double deltaTime)
        {
            CTX.Clear();

            CTX.SetDrawColor(1, 0, 0, 0.5f);

            CTX.DrawRect(20, 20, 100, 100);
            CTX.DrawCircle(500, 500, 200);
            CTX.DrawArc(200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            CTX.DrawArc(300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            CTX.DrawArc(400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            CTX.DrawLine(Window.Width - 60, 600, Window.Width - 100, 200, 10.0f, CapType.None);
            CTX.DrawLine(Window.Width - 100, 600, Window.Width - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            CTX.DrawLine(lineSize, lineSize, Window.Width - lineSize, Window.Height - lineSize, lineSize/2, CapType.Circle);


            CTX.SetDrawColor(0, 0, 1, 1f);

            CTX.DrawRectOutline(5, 20, 20, 100, 100);
            CTX.DrawCircleOutline(10, 500, 500, 200);
            CTX.DrawArcOutline(10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            CTX.DrawArcOutline(10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            CTX.DrawArcOutline(10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            CTX.DrawLineOutline(10, Window.Width - 60, 600, Window.Width - 100, 200, 10.0f, CapType.None);
            CTX.DrawLineOutline(10, Window.Width - 100, 600, Window.Width - 130, 200, 10.0f, CapType.Circle);

            CTX.DrawLineOutline(10, lineSize, lineSize, Window.Width - lineSize, Window.Height - lineSize, lineSize / 2, CapType.Circle);

            CTX.SwapBuffers();
        }
    }
}
