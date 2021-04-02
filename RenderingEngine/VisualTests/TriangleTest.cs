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
			
            _window.Size = (800, 600);
            _window.Title = "Triangle";
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

            CTX.DrawTriangle(_window.Width/2, 0, 0, 0,_window.Width/4, _window.Height/2);

            int lineSize = 100;
            CTX.DrawLine(lineSize, lineSize, _window.Width - lineSize, _window.Height - lineSize, lineSize/2, CapType.Circle);

            CTX.Flush();
        }



    }
}
