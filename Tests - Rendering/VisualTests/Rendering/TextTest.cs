using MinimalAF;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests
{
	class TextTest : IRenderable {
        List<string> rain;
        StringBuilder sb = new StringBuilder();
        Random rand = new Random();
        double timer = 0;
        float rainSpeed = 20;

        DrawableFont font;

        public TextTest(string fontName = "Consolas") {
            font = new DrawableFont(fontName, 16);
            rain = new List<string>();
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetTexture(font.Texture);
            ctx.SetDrawColor(0, 1, 0, 0.8f);

            timer += Time.DeltaTime * rainSpeed;
            if (timer > 1.0) {
                timer = 0;

                // add one line to the rain
                {
                    sb.Clear();

                    float totalLength = 0;
                    while (totalLength < ctx.VW) {
                        int character = rand.Next(0, 512);

                        char c = (char)character;
                        if (character > 126)    // TODO: unicode
                            c = ' ';

                        sb.Append(c);

                        totalLength += font.GetStringWidth("|");
                    }

                    rain.Insert(0, sb.ToString());
                    if ((rain.Count - 2) * font.GetStringHeight("|") > ctx.VH) {
                        rain.RemoveAt(rain.Count - 1);
                    }
                }
            }

            // idea being that we are pushing the rain down from 0 -> char height. When
            // the timer resets to zero and a new line is added to the start, this translate amount will
            // also jump back to zero, which will give the visual effect of smooth falling text
            // rather than staggered grid-snapped terminal text
            var translateAmount = font.GetStringHeight("|") * timer;
            ctx.SetModel(Matrix4.CreateTranslation(0, -((float)translateAmount), 0));

            for (int i = 0; i < rain.Count; i++) {
                font.DrawText(ctx, rain[i], 0, ctx.VH - font.GetStringHeight("|") * i);

                // nah we're using a whole ass matrix bc i feel like it
                // _font.Draw(ctx, rain[i], 0, -translateAmount + ctx.VH - ctx.GetCharHeight() * i);
            }
        }
    }
}
