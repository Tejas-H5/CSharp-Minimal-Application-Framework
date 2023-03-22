using MinimalAF;
using MinimalAF.Rendering;
using System;

namespace RenderingEngineVisualTests {
    class Benchmark : IRenderable {
        public float LineThickness {
            get; set;
        }
        public CapType CapType {
            get; set;
        }
        public int LineCount {
            get; set;
        }
        public float RequiredFPS {
            get; set;
        }

        public Benchmark(int lineCount = 0, CapType capType = CapType.Circle, int thickness = 5, float requiredFPS = 60) {
            LineThickness = thickness;
            CapType = capType;
            LineCount = lineCount;
            RequiredFPS = requiredFPS;
        }

        DrawableFont _font = new DrawableFont("Source code pro", 16);
        readonly Random rand = new Random(1);
        int frames = 0;
        double time = 0;
        int amount = 10000;
        readonly int jump = 6000;

        public void Render(FrameworkContext ctx) {
            time += Time.DeltaTime;
            frames++;

            double FPS = frames / time;

            if (time > 1f) {
                time = 0;
                frames = 0;

                if (LineCount > 0) {
                    amount = LineCount;
                } else {
                    double actualToWantedRatio = FPS / RequiredFPS;

                    amount = (int)Math.Ceiling(actualToWantedRatio * amount);
                    if (amount < 1) {
                        amount = 1;
                    }

                    if (Math.Abs(actualToWantedRatio - 1.0) < 0.01) {
                        Console.WriteLine("Converged at amount = " + amount + " lines required for " + RequiredFPS);
                    }
                }
            }


            ctx.SetDrawColor(1, 0, 0, 0.1f);
            CTX.TimesVertexThresholdReached = 0;
            CTX.TimesIndexThresholdReached = 0;

            for (int i = 0; i < amount; i++) {
                float x1 = ctx.VW * (float)rand.NextDouble();
                float y1 = ctx.VH * (float)rand.NextDouble();

                float x2 = ctx.VW * (float)rand.NextDouble();
                float y2 = ctx.VH * (float)rand.NextDouble();

                IM.Line(ctx, x1, y1, x2, y2, LineThickness, CapType);
            }

            ctx.SetDrawColor(0, 0, 0, 1f);

            string text = "FPS: " + FPS.ToString("0.000") +
                "\nLines drawn: " + amount +
                "\nCapType: " + CapType.ToString() +
                "\nVertex refreshes: " + CTX.TimesVertexThresholdReached +
                "\nIndex refreshes: " + CTX.TimesIndexThresholdReached +
                "\nVertex:Index Ratio: " + CTX.VertexToIndexRatio;

            text.ToCharArray();

            _font.Draw(ctx, text, 10, ctx.VH - 50);
        }
    }
}
