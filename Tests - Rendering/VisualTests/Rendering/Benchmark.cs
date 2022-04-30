using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
    [VisualTest(
        description: @"Performs gradient ascent to find the number of lines that can be drawn with the given parameters in order
to achieve some target FPS (only if count==0, otherwise the line count is fixed). Results may vary, but higher number of
lines drawn is always better. This test may not work well for when the target FPS is the same as the graphics card's max refresh rate,
in which case you will need to set it a bit lower. E.G if my graphics card will always refresh at 60FPS, I will
set the target FPS to 55 to make the test actually work.",
        tags: "2D"
    )]
    class Benchmark : Element
    {
        private int lineThiccness;
        CapType capType;
        int lineCount;
        float requiredFPS;

        public Benchmark(int lineCount = 0, CapType capType = CapType.Circle, int thickness = 5, float requiredFPS = 60)
        {
            lineThiccness = thickness;
            this.capType = capType;
            this.lineCount = lineCount;
            this.requiredFPS = requiredFPS;
        }

        public override void OnMount(Window w)
        {
            w.Size = (800, 600);
            w.Title = "Rendering Engine Line benchmark";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
            SetFont("Consolas", 24);
        }

        Random rand = new Random(1);

        int frames = 0;
        double time = 0;

        int amount = 10000;
        int jump = 6000;

        public override void OnRender()
        {
            time += Time.DeltaTime;
            frames++;

            double FPS = frames / time;

            if (time > 1f) {
                time = 0;
                frames = 0;

                if (lineCount > 0) {
                    amount = lineCount;
                } else {
                    double actualToWantedRatio = FPS / requiredFPS;

                    amount = (int)(actualToWantedRatio * amount);

                    if (Math.Abs(actualToWantedRatio - 1.0) < 0.01) {
                        Console.WriteLine("Converged at amount = " + amount + " lines required for " + requiredFPS);
                    }
                }
            }


            SetDrawColor(1, 0, 0, 0.1f);
            CTX.TimesVertexThresholdReached = 0;
            CTX.TimesIndexThresholdReached = 0;

            for (int i = 0; i < amount; i++)
            {
                float x1 = VW((float)rand.NextDouble());
                float y1 = VH((float)rand.NextDouble());

                float x2 = VW((float)rand.NextDouble());
                float y2 = VH((float)rand.NextDouble());

                DrawLine(x1, y1, x2, y2, lineThiccness, capType);
            }

            SetDrawColor(0, 0, 0, 1f);

            string text = "FPS: " + FPS.ToString("0.000") +
                "\nLines drawn: " + amount +
                "\nCapType: " + capType.ToString() + 
                "\nVertex refreshes: " + CTX.TimesVertexThresholdReached + 
                "\nIndex refreshes: " + CTX.TimesIndexThresholdReached + 
                "\nVertex:Index Ratio: " + CTX.VertexToIndexRatio;

            DrawText(text, 10, Height - 50);
        }
    }
}
