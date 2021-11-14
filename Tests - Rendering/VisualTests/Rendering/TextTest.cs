using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering
{
    class TextTest : Element
    {
        List<string> rain;

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Matrix rain test";

            CTX.SetClearColor(0, 0, 0, 0);

            CTX.SetCurrentFont("Consolas", 24);

            rain = new List<string>();
        }

        StringBuilder sb = new StringBuilder();
        Random rand = new Random();
        void PushGibberish()
        {
            sb.Clear();

            float totalLength = 0;
            while (totalLength < Width)
            {
                int character = rand.Next(0, 512);
                char c = (char)character;
                if (character > 126)
                    c = ' ';

                sb.Append(c);

                totalLength += CTX.GetCharWidth(c);
            }

            rain.Insert(0, sb.ToString());
            if ((rain.Count - 2) * CTX.GetCharHeight() > Height)
            {
                rain.RemoveAt(rain.Count - 1);
            }
        }


        double timer = 0;
        public override void OnUpdate()
        {
            //*
            timer += Time.DeltaTime;
            if (timer < 0.05)
                return;
            //*/
            timer = 0;

            PushGibberish();
        }

        public override void OnRender()
        {


            CTX.SetDrawColor(0, 1, 0, 0.8f);

            for (int i = 0; i < rain.Count; i++)
            {
                CTX.DrawText(rain[i], 0, Height - CTX.GetCharHeight() * i);
            }

            CTX.SetDrawColor(1, 0, 0, 1);

        }
    }
}
