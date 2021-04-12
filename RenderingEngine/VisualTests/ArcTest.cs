using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.Rendering.ImmediateMode;
using System;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class ArcTest : EntryPoint
    {
        public override void Start()
        {


            Window.Size = (800, 600);
            Window.Title = ("Arc Test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);
        }


        public override void Update(double deltaTime)
        {

        }

        float a;
        float b;

        public override void Render(double deltaTime)
        {


            CTX.SetDrawColor(1, 0, 0, 0.5f);

            float x0 = Window.Width / 2;
            float y0 = Window.Height / 2;
            float r = MathF.Min(Window.Height, Window.Width) * 0.45f;

            CTX.DrawArc(x0, y0, r, a, b);

            CTX.SetDrawColor(0, 0, 0, 0.5f);
            DrawHand(x0, y0, r, a);
            DrawHand(x0, y0, r, b);

            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.DrawText($"Angle a: {a}", 0, Window.Height - 30);
            CTX.DrawText($"Angle b: {b}", 0, Window.Height - 50);

            a += (float)deltaTime;
            b += (float)deltaTime * 2;


        }

        private void DrawHand(float x0, float y0, float r, float angle)
        {
            CTX.DrawLine(x0, y0, x0 + r * MathF.Sin(angle), y0 + r * MathF.Cos(angle), 15f, CapType.Circle);
        }



    }
}
