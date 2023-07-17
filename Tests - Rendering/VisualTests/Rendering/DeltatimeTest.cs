using MinimalAF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderingEngineVisualTests {
    internal class DeltatimeTest : IRenderable {
        double dtTimer = 0;
        Stopwatch _timer;
        DrawableFont _font = new DrawableFont("Source code pro", new DrawableFontOptions { });
        public DeltatimeTest() {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Render(AFContext ctx) {
            dtTimer += Time.DeltaTime;

            ctx.SetDrawColor(Color.Black);

            string dtTimerText = dtTimer.ToString(".000");
            string realTimerText = (_timer.ElapsedMilliseconds / 1000.0).ToString(".000");
            string text = "Delta time: " + dtTimerText + "\nReal time: " + realTimerText;
            _font.DrawText(ctx, text, new DrawTextOptions {
                X = ctx.VW * 0.5f, Y = ctx.VH * 0.5f,
                VAlign = 0.5f, HAlign = 0.5f
            });
        }
    }
}
