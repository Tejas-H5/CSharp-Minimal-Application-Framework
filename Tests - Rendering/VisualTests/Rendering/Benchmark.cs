using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    [VisualTest]
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

            if (time > 0.5f) {
                if (lineCount > 0) {
                    amount = lineCount;
                } else {
                    if (FPS < requiredFPS) {
                        amount -= jump;
                    } else if (FPS > requiredFPS) {
                        amount += jump;
                    }

                    jump /= 2;
                    if (jump == 0) {
                        Console.WriteLine($"Converged at {amount} lines required for {requiredFPS}");

                        jump = 1000;
                    }
                }

                time = 0;
                frames = 0;
            }


            SetDrawColor(1, 0, 0, 0.1f);

            for (int i = 0; i < amount; i++)
            {
                float x1 = VW((float)rand.NextDouble());
                float y1 = VH((float)rand.NextDouble());

                float x2 = VW((float)rand.NextDouble());
                float y2 = VH((float)rand.NextDouble());

                Line(x1, y1, x2, y2, lineThiccness, capType);
            }

            SetDrawColor(0, 0, 0, 1f);

            string text = "FPS: " + FPS.ToString("0.000") +
                "\nLines drawn: " + amount +
                "\nCapType: " + capType.ToString();

            Text(text, 10, Height - 50);
        }
    }
}
