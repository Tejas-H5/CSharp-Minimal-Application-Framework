using MinimalAF.Rendering;
using MinimalAF;
using OpenTK.Mathematics;
using System;

namespace RenderingEngineVisualTests
{
	[VisualTest(
        description: @"Test that texture loading works. Also test that transforms work, cause why not 
(there are actually reasons why not, but I dont rlly care at the moment).
The texture on the right is with nearest neighbour, and the texture on the left is with bilinear, so it's a bit blurry on the edges).
Also, the texture on the left must be still, while only the one on the right is moving",
        tags: "2D, texture"
    )]
	class TextureTest : IRenderable {
        Texture tex;
        Texture tex2;

        public TextureTest(FrameworkContext ctx) {
            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "Texture loading test";
            w.RenderFrequency = 120; // 60;
            w.UpdateFrequency = 120; // 120;

            ctx.SetClearColor(Color.White);

            TextureMap.Load("placeholder", "./Res/settings_icon.png", new TextureImportSettings());
            TextureMap.Load("placeholderNN", "./Res/settings_icon.png", new TextureImportSettings { Filtering = FilteringType.NearestNeighbour });

            tex = TextureMap.Get("placeholder");
            tex2 = TextureMap.Get("placeholderNN");
        }

        float t = 0;

        public void Render(FrameworkContext ctx) {
            t += Time.DeltaTime;

            ctx.SetDrawColor(1, 1, 1, 1);
            ctx.SetTexture(tex);

            ctx.DrawRect(20, 20, ctx.VW / 2 - 20, ctx.VH - 20);
            Rect rect2 = new Rect(ctx.VW / 2 + 20, 20, ctx.VW - 20, ctx.VH - 20);


            ctx.SetTransform(
                Matrix4.CreateScale(MathF.Sin(t)) *
                Matrix4.CreateRotationZ(t) *
                Matrix4.CreateTranslation(rect2.CenterX, rect2.CenterY, 0)
            );

			ctx.SetTexture(tex2);
			ctx.DrawRect(new Rect(-rect2.Width / 2, -rect2.Height / 2, rect2.Width / 2, rect2.Height / 2).Rectified());

            ctx.Use();

            Rect centerRect = new Rect(ctx.VW * 0.5f - tex.Width / 2, ctx.VH * 0.5f - tex.Height / 2, ctx.VW * 0.5f + tex.Width / 2, ctx.VH * 0.5f + tex.Height / 2);
            ctx.DrawRect(centerRect);
        }
    }
}
