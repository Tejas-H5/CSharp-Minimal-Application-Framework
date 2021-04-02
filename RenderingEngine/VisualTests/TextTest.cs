using OpenTK.Mathematics;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Rendering;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class TextTest : EntryPoint
    {
        List<string> rain;

        public override void Start()
        {
			
            _window.Size=(800, 600);
            _window.Title=("Texture loading test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            RenderingContext.SetClearColor(0,0,0,0);

            RenderingContext.SetCurrentFont("Consolas", 24);

            _window.MouseWheel += MousewheelScroll;

            rain = new List<string>();
        }

        StringBuilder sb = new StringBuilder();
        Random rand = new Random();
        void PushGibberish()
        {
            sb.Clear();

            float totalLength = 0;
            while(totalLength < _window.Width)
            {
                int character = rand.Next(0, 512);
                char c = (char)character;
                if (character > 126)
                    c = ' ';

                sb.Append(c);

                totalLength += RenderingContext.GetCharWidth(c);
            }

            rain.Insert(0, sb.ToString());
            if((rain.Count-2) * RenderingContext.GetCharHeight() > _window.Height)
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

            PushGibberish();
        }

        float pos = 0;

        public override void Render(double deltaTime)
        {
            RenderingContext.Clear();

            RenderingContext.SetDrawColor(0, 1, 0, 0.8f);

            for(int i = 0; i < rain.Count; i++)
            {
                RenderingContext.DrawText(rain[i], 0, _window.Height - RenderingContext.GetCharHeight() * i);
            }

            RenderingContext.SetDrawColor(1, 0, 0, 1);

            RenderingContext.Flush();
        }



    }
}
