using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    class GeometryAndTextTest : IRenderable {
        List<string> rain = new List<string>();
        Texture tex;
        TextTest textTest;

        public GeometryAndTextTest() {
            TextureMap.Load("placeholder", "./Res/settings_icon.png", new TextureImportSettings {
                Filtering = FilteringType.NearestNeighbour
            });

            tex = TextureMap.Get("placeholder");
            textTest = new TextTest();
        }

        float a = 0;
        public void Render(FrameworkContext ctx) {
            a += (float)Time.DeltaTime;

            ctx.SetTexture(null);
            ctx.SetDrawColor(Color.RGBA(0, 1, 0, 0.5f));
            IM.DrawArc(ctx, ctx.VW / 2, ctx.VH / 2, MathF.Min(ctx.VH / 2f, ctx.VW / 2f), a, MathF.PI * 2 + a, 6);

            ctx.SetTexture(tex);

            IM.DrawRect(ctx, ctx.VW * 0.5f - 50, ctx.VH * 0.5f - 50, ctx.VW * 0.5f + 50, ctx.VH * 0.5f + 50);
            IM.DrawRect(ctx, 100, 100, ctx.VW - 100, ctx.VH - 100);
        }
    }
}
