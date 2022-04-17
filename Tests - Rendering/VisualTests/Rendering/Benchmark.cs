using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    [VisualTest]
    class Benchmark : Element
    {
        private int _lineThiccness;
        CapType _capType;

        public Benchmark(CapType capType = CapType.Circle, int thickness = 5)
        {
            _lineThiccness = thickness;
            _capType = capType;
        }

        public override void OnMount(Window w)
        {
            w.Size = (800, 600);
            w.Title = "Rendering Engine Line benchmark";

           SetClearColor(Color4.RGBA(1, 1, 1, 1));
            SetFont("Consolas", 24);
        }

        Random rand = new Random(1);

        public override void OnUpdate()
        {
        }

        int frames = 0;
        double time = 0;

        int amount = 10000;
        int jump = 6000;

        public override void OnRender()
        {
            SetDrawColor(1, 0, 0, 0.1f);

            for (int i = 0; i < amount; i++)
            {
                float x1 = VW((float)rand.NextDouble());
                float y1 = VH((float)rand.NextDouble());

                float x2 = VW((float)rand.NextDouble());
                float y2 = VH((float)rand.NextDouble());

                Line(x1, y1, x2, y2, _lineThiccness, _capType);
            }

            double FPS = frames / time;
            SetDrawColor(0, 0, 0, 1f);

            string text = "FPS: " + FPS.ToString("0.000") +
                "\nLines drawn: " + amount + 
                "\nCapType: " + _capType.ToString();

            Text(text, 10, Height - 50);

            time += Time.DeltaTime;
            frames++;

            float requiredFPS = 60;
            if (time > 0.5f)
            {
                if (FPS < requiredFPS)
                {
                    amount -= jump;
                }
                else if (FPS > requiredFPS)
                {
                    amount += jump;
                }

                jump /= 2;
                if (jump == 0)
                {
                    Console.WriteLine($"Converged at {amount} lines required for {requiredFPS}");

                    jump = 1000;
                }

                time = 0;
                frames = 0;
            }

            //RenderingContext.DrawLine(-size, -size, size, size, 0.02f, CapType.Circle);

            //RenderingContext.DrawFilledArc(Width/2, Height/2, size, 0, MathF.PI * 2);
        }
    }
}
