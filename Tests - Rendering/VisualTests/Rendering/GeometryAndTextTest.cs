using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Tests that text rendering, geometry rendering and normal texture rendering can happen
all at once.",
        tags: "2D"
    )]
    class GeometryAndTextTest : IRenderable {
        List<string> rain = new List<string>();
        Texture tex;
        TextTest textTest;

        public GeometryAndTextTest(FrameworkContext ctx) {
            TextureMap.Load("placeholder", "./Res/settings_icon.png", new TextureImportSettings {
                Filtering = FilteringType.NearestNeighbour
            });

            tex = TextureMap.Get("placeholder");
            textTest = new TextTest(new FrameworkContext { });

            if (ctx.Window == null) return;

            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "Text and geometry test";

            ctx.SetClearColor(Color.White);
        }

        float a = 0;
        public void Render(FrameworkContext ctx) {
            a += (float)Time.DeltaTime;

            ctx.SetTexture(null);
            ctx.SetDrawColor(Color.RGBA(0, 1, 0, 0.5f));
            ctx.DrawArc(ctx.VW / 2, ctx.VH / 2, MathF.Min(ctx.VH / 2f, ctx.VW / 2f), a, MathF.PI * 2 + a, 6);

            ctx.SetTexture(tex);

            ctx.DrawRect(ctx.VW * 0.5f - 50, ctx.VH * 0.5f - 50, ctx.VW * 0.5f + 50, ctx.VH * 0.5f + 50);
            ctx.DrawRect(100,100,ctx.VW-100, ctx.VH-100);
        }
    }
}
