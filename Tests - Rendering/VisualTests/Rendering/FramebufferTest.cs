﻿using MinimalAF;
using MinimalAF.Rendering;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests {
    public class FramebufferTest : IRenderable {
        Framebuffer fb;

        public FramebufferTest() {
            fb = new Framebuffer(1, 1);
            fb.Resize(800, 600);
        }

        public void Render(AFContext ctx) {
            float wCX = 400;
            float wCY = 300;

            ctx.UseFramebuffer(fb);
            {
                ctx.Clear(Color.RGBA(0, 0, 0, 0));

                ctx.SetProjectionCartesian2D(1, 1, 0, 0);
                ctx.SetTransform(Matrix4.CreateTranslation(0, 0, 0));

                ctx.SetDrawColor(0, 0, 1, 1);
                DrawDualCirclesCenter(ref ctx, wCX, wCY);

                ctx.SetDrawColor(1, 1, 0, 1);
                IM.DrawRect(ctx, wCX, wCY, wCX + 50, wCY + 25);
            }
            ctx.UseFramebuffer(null);
            ctx.Use();

            ctx.SetDrawColor(1, 0, 0, 1);
            float rectSize = 200;
            IM.DrawRect(ctx, wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);

            ctx.SetTexture(fb.Texture);
            ctx.SetDrawColor(1, 1, 1, 0.5f);
            IM.DrawRect(ctx, 0, 0, 800, 600);

            ctx.SetTexture(null);

            ctx.SetDrawColor(0, 1, 0, 0.5f);
            IM.DrawRectOutline(ctx, 10, wCX - wCY, wCY - wCY, wCX + wCY, wCY + wCY);
        }

        private void DrawDualCirclesCenter(ref AFContext ctx, float x, float y) {
            IM.DrawCircle(ctx, x - 100, y - 100, 200);
            IM.DrawCircle(ctx, x + 100, y + 100, 200);
        }
    }
}
