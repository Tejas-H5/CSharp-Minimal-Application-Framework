using MinimalAF;
using System;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Test that the stencilling functionality is working.
The red square must appear above the blue square, and there should be vertical bars that retract and extend, masking
the visibility of another test.",
        tags: "2D, stencil"
    )]
    public class StencilTest : IRenderable {
        GeometryAndTextTest geometryAndTextTest;

        public StencilTest(FrameworkContext ctx) {
            geometryAndTextTest = new GeometryAndTextTest(new FrameworkContext());

            if (ctx.Window == null) return;

            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "Stencil rendering test";

            ctx.SetClearColor(Color.White);
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
