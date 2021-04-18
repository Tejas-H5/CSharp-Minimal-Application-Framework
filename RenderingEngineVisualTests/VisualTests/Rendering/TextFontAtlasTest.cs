using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests.Rendering
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class TextFontAtlasText : EntryPoint
    {
        List<string> rain;

        public override void Start()
        {

            Window.Size = (800, 600);
            Window.Title = "text font atlas test";
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(0, 0, 0, 0);

            CTX.SetCurrentFont("Consolas", 24);

            Window.MouseWheel += MousewheelScroll;
        }

        private void MousewheelScroll(float amount)
        {
            pos += 50 * amount;
        }


        double timer = 0;
        public override void Update(double deltaTime)
        {
            //*
            timer += deltaTime;
            if (timer < 0.05)
                return;
            //*/
            timer = 0;
        }

        float pos = 0;

        public override void Render(double deltaTime)
        {
            //Draw font atlas offset by vertical scroll in pos variable
        }
    }
}
