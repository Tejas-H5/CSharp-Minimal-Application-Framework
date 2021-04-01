using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Rendering;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class ArcTest : EntryPoint
    {
        public override void Start(RenderingContext ctx, GraphicsWindow window)
        {
            base.Start(ctx, window);

            window.Size=(800, 600);
            window.Title=("Arc Test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            ctx.SetClearColor(1, 1, 1, 1);
        }


        public override void Update(double deltaTime)
        {

        }

        float a;
        float b;

        public override void Render(double deltaTime)
        {
            ctx.Clear();

            ctx.SetDrawColor(1, 0, 0, 0.5f);

            float x0 = window.Width / 2;
            float y0 = window.Height / 2;
            float r = MathF.Min(window.Height, window.Width) * 0.45f;

            ctx.DrawFilledArc(x0, y0, r, a, b);

            ctx.SetDrawColor(0, 0, 0, 0.5f);
            DrawHand(ctx, x0, y0, r, a);
            DrawHand(ctx, x0, y0, r, b);

            ctx.SetDrawColor(0, 0, 0, 1);
            ctx.DrawText($"Angle a: {a}", 0, window.Height-30);
            ctx.DrawText($"Angle b: {b}", 0, window.Height-50);

            a += (float)deltaTime;
            b += (float)deltaTime * 2;

            ctx.Flush();
        }

        private void DrawHand(RenderingContext ctx, float x0, float y0, float r, float angle)
        {
            ctx.DrawLine(x0, y0, x0 + r * MathF.Sin(angle), y0 + r * MathF.Cos(angle), 15f, CapType.Circle);
        }



    }
}
