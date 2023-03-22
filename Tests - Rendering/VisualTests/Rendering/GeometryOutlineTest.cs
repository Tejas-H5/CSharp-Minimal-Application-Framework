using MinimalAF;
using MinimalAF.Rendering;
using System;

namespace RenderingEngineVisualTests {
    class GeometryOutlineTest : IRenderable {
        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(1, 0, 0, 0.5f);

            IM.Rect(ctx, 20, 20, 100, 100);
            IM.Circle(ctx, 500, 500, 200);
            IM.Arc(ctx, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            IM.Arc(ctx, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            IM.Arc(ctx, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            IM.Line(ctx, ctx.VW - 60, 600, ctx.VW - 100, 200, 10.0f, CapType.None);
            IM.Line(ctx, ctx.VW - 100, 600, ctx.VW - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            IM.Line(ctx, lineSize, lineSize, ctx.VW - lineSize, ctx.VH - lineSize, lineSize / 2, CapType.Circle);


            ctx.SetDrawColor(0, 0, 1, 1f);

            IM.RectOutline(ctx, 5, 20, 20, 100, 100);
            IM.CircleOutline(ctx, 10, 500, 500, 200);
            IM.ArcOutline(ctx, 10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            IM.ArcOutline(ctx, 10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            IM.ArcOutline(ctx, 10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            IM.LineOutline(ctx, 10, ctx.VW - 60, 600, ctx.VW - 100, 200, 10.0f, CapType.None);
            IM.LineOutline(ctx, 10, ctx.VW - 100, 600, ctx.VW - 130, 200, 10.0f, CapType.Circle);

            IM.LineOutline(ctx, 10, lineSize, lineSize, ctx.VW - lineSize, ctx.VH - lineSize, lineSize / 2, CapType.Circle);
        }
    }
}
