using OpenTK.Mathematics;
using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class TextTest : EntryPoint
    {
        List<string> rain;

        public override void Start(RenderingContext ctx, GraphicsWindow window)
        {
			base.Start(ctx, window);
            window.Size=(800, 600);
            window.Title=("Texture loading test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            ctx.SetClearColor(0,0,0,0);

            ctx.SetCurrentFont("Consolas", 24);

            window.MouseWheel += MousewheelScroll;

            rain = new List<string>();
        }

        StringBuilder sb = new StringBuilder();
        Random rand = new Random();
        void PushGibberish(RenderingContext ctx)
        {
            sb.Clear();

            float totalLength = 0;
            while(totalLength < window.Width)
            {
                int character = rand.Next(0, 512);
                char c = (char)character;
                if (character > 126)
                    c = ' ';

                sb.Append(c);

                totalLength += ctx.GetCharWidth(c);
            }

            rain.Insert(0, sb.ToString());
            if((rain.Count-2) * ctx.GetCharHeight() > window.Height)
            {
                rain.RemoveAt(rain.Count - 1);
            }
        }

        private void MousewheelScroll(OpenTK.Windowing.Common.MouseWheelEventArgs obj)
        {
            pos += 50 * obj.OffsetY;
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

            PushGibberish(ctx);
        }

        float pos = 0;

        public override void Render(double deltaTime)
        {
            ctx.Clear();

            ctx.SetDrawColor(0, 1, 0, 0.8f);

            for(int i = 0; i < rain.Count; i++)
            {
                ctx.DrawText(rain[i], 0, window.Height - ctx.GetCharHeight() * i);
            }

            ctx.SetDrawColor(1, 0, 0, 1);

            ctx.Flush();
        }



    }
}
