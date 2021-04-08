using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Rendering;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class Benchmark : EntryPoint
    {
        private int _lineThiccness;
        public Benchmark(int thickness)
        {
            _lineThiccness = thickness;
        }

        public override void Start()
        {
			
            Window.Size=(800, 600);
            Window.Title=("Rendering Engine Line benchmark");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);
            CTX.SetCurrentFont("Consolas", 24);
        }

        Random rand = new Random(1);

        public override void Update(double deltaTime)
        {

        }

        int frames = 0;
        double time = 0;

        int amount = 10000;
        int jump = 6000;

        public override void Render(double deltaTime)
        {
           

            CTX.SetDrawColor(1, 0, 0, 0.1f);

            for (int i = 0; i < amount; i++)
            {
                float x1 = (float)rand.NextDouble() * Window.Width;
                float y1 = (float)rand.NextDouble() * Window.Height;

                float x2 = (float)rand.NextDouble() * Window.Width;
                float y2 = (float)rand.NextDouble() * Window.Height;

                CTX.DrawLine(x1, y1, x2, y2, _lineThiccness, CapType.Circle);
            }


            double FPS = frames / time;
            CTX.SetDrawColor(0, 0, 0, 1f);
            CTX.DrawText($"FPS: {FPS.ToString("0.000")}", 10, Window.Height-50);
            CTX.DrawText($"Lines drawn: {amount}", 10, Window.Height - 100);

            time += deltaTime;
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

            //RenderingContext.DrawFilledArc(window.Width/2, window.Height/2, size, 0, MathF.PI * 2);
            
        }



    }
}
