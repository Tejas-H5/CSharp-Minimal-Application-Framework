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

        public DeltatimeTest() {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Render(FrameworkContext ctx) {
            dtTimer += Time.DeltaTime;

            ctx.SetFont("Source code PRO", 24);
            ctx.SetDrawColor(Color.Black);

            string dtTimerText = dtTimer.ToString(".000");
            string realTimerText = (_timer.ElapsedMilliseconds / 1000.0).ToString(".000");
            string text = "Delta time: " + dtTimerText + "\nReal time: " + realTimerText;
            ctx.DrawText(text, ctx.VW * 0.5f, ctx.VH * 0.5f, HAlign.Center, VAlign.Center);
        }
    }
}
