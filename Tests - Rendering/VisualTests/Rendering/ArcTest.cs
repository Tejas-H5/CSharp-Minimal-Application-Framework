using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	class ArcTest : Element
    {
        public override void OnStart()
        {
            WindowElement w = GetAncestor<WindowElement>();
            w.Size = (800, 600);
            w.Title = "Arc Test";

            CTX.SetClearColor(1, 1, 1, 1);
        }

        public override void OnUpdate()
        {
        }

        float a;
        float b;

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 0, 0, 0.5f);

            float x0 = Rect.CenterX;
            float y0 = Rect.CenterY;
            float r = MathF.Min(Height, Width) * 0.45f;

            CTX.Arc.Draw(x0, y0, r, a, b);

            CTX.SetDrawColor(0, 0, 0, 0.5f);
            DrawHand(x0, y0, r, a);
            DrawHand(x0, y0, r, b);

            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.Text.Draw($"Angle a: {a}", 0, Height - 30);
            CTX.Text.Draw($"Angle b: {b}", 0, Height - 50);

            a += (float)Time.DeltaTime;
            b += (float)Time.DeltaTime * 2;
        }

        private void DrawHand(float x0, float y0, float r, float angle)
        {
            CTX.Line.Draw(x0, y0, x0 + r * MathF.Sin(angle), y0 + r * MathF.Cos(angle), 15f, CapType.Circle);
        }
    }
}
