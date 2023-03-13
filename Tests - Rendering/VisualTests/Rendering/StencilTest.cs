﻿using MinimalAF;
using System;

namespace RenderingEngineVisualTests {
    public class StencilTest : IRenderable {
        GeometryAndTextTest geometryAndTextTest;

        public StencilTest() {
            geometryAndTextTest = new GeometryAndTextTest();
        }

        float xPos = 0;
        float time;

        public void Render(FrameworkContext ctx) {
            time += (float)Time.DeltaTime;
            xPos = 200 * MathF.Sin(time / 2.0f);

            ctx.SetDrawColor(1, 1, 1, 1);
            ctx.DrawText("Stencil test", 0, ctx.VH, HAlign.Left, VAlign.Top);

            ctx.StartStencillingWithoutDrawing(true);

            float barSize = MathF.Abs((ctx.VH / 2 - 5) * MathF.Sin(time / 4f));
            ctx.DrawRect(0, ctx.VH - barSize, ctx.VW, ctx.VH);
            ctx.DrawRect(0, 0, ctx.VW, barSize);

            ctx.StartUsingStencil();
            {
                geometryAndTextTest.Render(ctx);
            }
            // TODO low priority: make stencilling stack based.
            // Any of the children elements could have called this and made wierd stuff happen
            ctx.StopUsingStencil();

            ctx.StartStencillingWhileDrawing();

            float size = 60;
            DrawRedRectangle(ref ctx, size, xPos);

            ctx.StartUsingStencil();

            size = 70;
            DrawBlueRectangle(ref ctx, size, xPos);

            ctx.StopUsingStencil();
        }

        private void DrawBlueRectangle(ref FrameworkContext ctx, float size, float xPos) {
            ctx.SetTexture(null);
            ctx.SetDrawColor(0, 0, 1, 1);
            ctx.DrawRect(ctx.VW / 2 - size + xPos, ctx.VH / 2 - size,
                ctx.VW / 2 + size + xPos, ctx.VH / 2 + size);
        }

        private void DrawRedRectangle(ref FrameworkContext ctx, float size, float xPos) {
            ctx.SetTexture(null);
            ctx.SetDrawColor(1, 0, 0, 1);
            ctx.DrawRect(ctx.VW / 2 - size + xPos, ctx.VH / 2 - size,
                ctx.VW / 2 + size + xPos, ctx.VH / 2 + size);
        }

    }
}
