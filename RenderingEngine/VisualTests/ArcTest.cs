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
        public override void Start()
        {
            

            _window.Size=(800, 600);
            _window.Title=("Arc Test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            _ctx.SetClearColor(1, 1, 1, 1);
        }


        public override void Update(double deltaTime)
        {

        }

        float a;
        float b;

        public override void Render(double deltaTime)
        {
            _ctx.Clear();

            _ctx.SetDrawColor(1, 0, 0, 0.5f);

            float x0 = _window.Width / 2;
            float y0 = _window.Height / 2;
            float r = MathF.Min(_window.Height, _window.Width) * 0.45f;

            _ctx.DrawFilledArc(x0, y0, r, a, b);

            _ctx.SetDrawColor(0, 0, 0, 0.5f);
            DrawHand(_ctx, x0, y0, r, a);
            DrawHand(_ctx, x0, y0, r, b);

            _ctx.SetDrawColor(0, 0, 0, 1);
            _ctx.DrawText($"Angle a: {a}", 0, _window.Height-30);
            _ctx.DrawText($"Angle b: {b}", 0, _window.Height-50);

            a += (float)deltaTime;
            b += (float)deltaTime * 2;

            _ctx.Flush();
        }

        private void DrawHand(RenderingContext ctx, float x0, float y0, float r, float angle)
        {
            ctx.DrawLine(x0, y0, x0 + r * MathF.Sin(angle), y0 + r * MathF.Cos(angle), 15f, CapType.Circle);
        }



    }
}
