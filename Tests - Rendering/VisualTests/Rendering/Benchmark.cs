using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	//Performs a binary search to see the max number of random lines that can be drawn for 60FPS
	class Benchmark : Element
    {
        private int _lineThiccness;
        public Benchmark(int thickness)
        {
            _lineThiccness = thickness;
        }

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Rendering Engine Line benchmark";

            ClearColor = Color4.RGBA(1, 1, 1, 1);
            CTX.Text.SetFont("Consolas", 24);
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
            CTX.SetDrawColor(1, 0, 0, 0.1f);

            for (int i = 0; i < amount; i++)
            {
                float x1 = Left + (float)rand.NextDouble() * Width;
                float y1 = Bottom + (float)rand.NextDouble() * Height;

                float x2 = Left + (float)rand.NextDouble() * Width;
                float y2 = Bottom + (float)rand.NextDouble() * Height;

                CTX.Line.Draw(x1, y1, x2, y2, _lineThiccness, CapType.Circle);
            }

            double FPS = frames / time;
            CTX.SetDrawColor(0, 0, 0, 1f);
            CTX.Text.Draw($"FPS: {FPS.ToString("0.000")}", Left + 10, Bottom + Height - 50);
            CTX.Text.Draw($"Lines drawn: {amount}", Left + 10, Bottom + Height - 100);

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
