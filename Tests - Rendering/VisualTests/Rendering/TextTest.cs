using MinimalAF;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests
{
	[VisualTest(
        description: @"Test the text rendering, and other functions that detect the text width and height",
        tags: "2D, text"
    )]
	class TextTest : IRenderable {
        List<string> rain;
        string font;
        StringBuilder sb = new StringBuilder();
        Random rand = new Random();

        public TextTest(FrameworkContext ctx, string font = "Consolas") {
            this.font = font;
            rain = new List<string>();

            if (ctx.Window == null) return;

            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "Matrix rain test";

            ctx.SetClearColor(Color.White);
        }

        void PushGibberish(ref FrameworkContext ctx)
        {
            sb.Clear();

            float totalLength = 0;
            while (totalLength < ctx.VW)
            {
                int character = rand.Next(0, 512);
				//int character = rand.Next(0, 144697);
				char c = (char)character;
                if (character > 126)
                    c = ' ';

                sb.Append(c);

                totalLength += ctx.GetCharWidth(c);
            }

            rain.Insert(0, sb.ToString());
            if ((rain.Count - 2) * ctx.GetCharHeight() > ctx.VH) {
                rain.RemoveAt(rain.Count - 1);
            }
        }

        double timer = 0;
        public void Render(FrameworkContext ctx) {
            // if (ctx.IsUpdate) {
            timer += Time.DeltaTime;
            if (timer > 0.05) {
                timer = 0;
                PushGibberish(ref ctx);
            }
            // }

            ctx.SetFont(font, 16);
            ctx.SetDrawColor(0, 1, 0, 0.8f);

            for (int i = 0; i < rain.Count; i++) {
                ctx.DrawText(rain[i], 0, ctx.VH - ctx.GetCharHeight() * i);
            }
        }
    }
}
