using MinimalAF;
using MinimalAF.Rendering;
using System;

namespace RenderingEngineVisualTests {
    class GeometryOutlineTest : IRenderable {
        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(1, 0, 0, 0.5f);

            ctx.DrawRect(20, 20, 100, 100);
            ctx.DrawCircle(500, 500, 200);
            ctx.DrawArc(200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            ctx.DrawArc(300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            ctx.DrawArc(400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            ctx.DrawLine(ctx.VW - 60, 600, ctx.VW - 100, 200, 10.0f, CapType.None);
            ctx.DrawLine(ctx.VW - 100, 600, ctx.VW - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            ctx.DrawLine(lineSize, lineSize, ctx.VW - lineSize, ctx.VH - lineSize, lineSize / 2, CapType.Circle);


            ctx.SetDrawColor(0, 0, 1, 1f);

            ctx.DrawRectOutline(5, 20, 20, 100, 100);
            ctx.DrawCircleOutline(10, 500, 500, 200);
            ctx.DrawArcOutline(10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            ctx.DrawArcOutline(10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            ctx.DrawArcOutline(10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            ctx.DrawLineOutline(10, ctx.VW - 60, 600, ctx.VW - 100, 200, 10.0f, CapType.None);
            ctx.DrawLineOutline(10, ctx.VW - 100, 600, ctx.VW - 130, 200, 10.0f, CapType.Circle);

            ctx.DrawLineOutline(10, lineSize, lineSize, ctx.VW - lineSize, ctx.VH - lineSize, lineSize / 2, CapType.Circle);
        }
    }
}
