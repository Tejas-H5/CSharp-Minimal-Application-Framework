using MinimalAF;
using MinimalAF.Rendering;
using System;

namespace RenderingEngineVisualTests {
    class ArcTest : IRenderable {
        float a;
        float b;

        DrawableFont _font = new DrawableFont("Source code pro", new DrawableFontOptions { });

        public void Render(AFContext ctx) {
            ctx.SetDrawColor(1, 0, 0, 0.5f);

            float x0 = ctx.VW * 0.5f;
            float y0 = ctx.VH * 0.5f;
            float r = MathF.Min(ctx.VH, ctx.VW) * 0.45f;

            IM.DrawArc(ctx, x0, y0, r, a, b);

            ctx.SetDrawColor(0, 0, 0, 0.5f);
            DrawHand(ref ctx, x0, y0, r, a);
            DrawHand(ref ctx, x0, y0, r, b);

            ctx.SetDrawColor(0, 0, 0, 1);
            _font.DrawText(ctx, $"Angle a: {a}\nAngle b: {b}" + a, new DrawTextOptions {
                FontSize = 16, X = 0, Y = ctx.VH, VAlign=1
            });

            a += Time.DeltaTime;
            b += Time.DeltaTime * 2f;
        }

        private void DrawHand(ref AFContext ctx, float x0, float y0, float r, float angle) {
            IM.DrawLine(ctx, x0, y0, x0 + r * MathF.Sin(angle), y0 + r * MathF.Cos(angle), 15f, CapType.Circle);
        }
    }
}
