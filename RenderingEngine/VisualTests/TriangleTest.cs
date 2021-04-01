using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class TriangleTest : EntryPoint
    {
        public override void Start(RenderingContext ctx, GraphicsWindow window)
        {
			base.Start(ctx, window);
            window.Size = (800, 600);
            window.Title = "Triangle";
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            ctx.SetClearColor(0, 1, 1, 1);
        }


        public override void Update(double deltaTime)
        {

        }

        public override void Render(double deltaTime)
        {
            ctx.Clear();

            ctx.SetDrawColor(1, 0, 0, 1);

            ctx.DrawTriangle(window.Width/2, 0, 0, 0,window.Width/4, window.Height/2);

            int lineSize = 100;
            ctx.DrawLine(lineSize, lineSize, window.Width - lineSize, window.Height - lineSize, lineSize/2, CapType.Circle);

            ctx.Flush();
        }



    }
}
