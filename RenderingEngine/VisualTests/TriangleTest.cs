using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Rendering;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class TriangleTest : EntryPoint
    {
        public override void Start()
        {
			
            Window.Size = (800, 600);
            Window.Title = "Triangle";
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(0, 1, 1, 1);
        }


        public override void Update(double deltaTime)
        {

        }

        public override void Render(double deltaTime)
        {
            CTX.Clear();

            CTX.SetDrawColor(1, 0, 0, 1);

            CTX.DrawTriangle(Window.Width/2, 0, 0, 0,Window.Width/4, Window.Height/2);

            int lineSize = 100;
            CTX.DrawLine(lineSize, lineSize, Window.Width - lineSize, Window.Height - lineSize, lineSize/2, CapType.Circle);

            CTX.Flush();
        }



    }
}
