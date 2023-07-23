using MinimalAF.Rendering;
using MinimalAF;
using OpenTK.Mathematics;
using System;

namespace RenderingEngineVisualTests
{
	class TextureTest : IRenderable {
        Texture tex;
        Texture tex2;

        public TextureTest() {
            TextureMap.Load("placeholder", "./Res/settings_icon.png", new TextureImportSettings());
            TextureMap.Load("placeholderNN", "./Res/settings_icon.png", new TextureImportSettings { Filtering = FilteringType.NearestNeighbour });

            tex = TextureMap.Get("placeholder");
            tex2 = TextureMap.Get("placeholderNN");
        }

        float t = 0;

        public void Render(AFContext ctx) {
            t += Time.DeltaTime;

            ctx.SetDrawColor(1, 1, 1, 1);
            ctx.SetTexture(tex);

            IM.DrawRect(ctx, 20, 20, ctx.VW / 2 - 20, ctx.VH - 20);
            Rect rect2 = new Rect(ctx.VW / 2 + 20, 20, ctx.VW - 20, ctx.VH - 20);


            ctx.SetTransform(
                Matrix4.CreateScale(MathF.Sin(t)) *
                Matrix4.CreateRotationZ(t) *
                Matrix4.CreateTranslation(rect2.CenterX, rect2.CenterY, 0)
            );

			ctx.SetTexture(tex2);
			IM.DrawRect(ctx, new Rect(-rect2.Width / 2, -rect2.Height / 2, rect2.Width / 2, rect2.Height / 2).Rectified());

            ctx.Use();

            Rect centerRect = new Rect(ctx.VW * 0.5f - tex.Width / 2, ctx.VH * 0.5f - tex.Height / 2, ctx.VW * 0.5f + tex.Width / 2, ctx.VH * 0.5f + tex.Height / 2);
            IM.DrawRect(ctx, centerRect);
        }
    }
}
