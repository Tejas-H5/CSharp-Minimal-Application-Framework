using MinimalAF;
using MinimalAF.Rendering;
using System;

namespace RenderingEngineVisualTests {
    class ArcTest : IRenderable {
        float a;
        float b;

        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(1, 0, 0, 0.5f);

            float x0 = ctx.VW * 0.5f;
            float y0 = ctx.VH * 0.5f;
            float r = MathF.Min(ctx.VH, ctx.VW) * 0.45f;

            ctx.DrawArc(x0, y0, r, a, b);

            ctx.SetDrawColor(0, 0, 0, 0.5f);
            DrawHand(ref ctx, x0, y0, r, a);
            DrawHand(ref ctx, x0, y0, r, b);

            ctx.SetDrawColor(0, 0, 0, 1);
            ctx.DrawText("Angle a: " + a, 0, ctx.VH - 30);
            ctx.DrawText("Angle b: " + b, 0, ctx.VH - 50);

            a += Time.DeltaTime;
            b += Time.DeltaTime * 2f;
        }

        private void DrawHand(ref FrameworkContext ctx, float x0, float y0, float r, float angle) {
            ctx.DrawLine(x0, y0, x0 + r * MathF.Sin(angle), y0 + r * MathF.Cos(angle), 15f, CapType.Circle);
        }
    }
}
