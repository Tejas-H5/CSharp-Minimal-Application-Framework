using MinimalAF;
using MinimalAF.Rendering;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests {
    [VisualTest(
    description: @"Tests that framebuffers can be used properly.
The red square must be fully visible under the circles.
The part where the circles overlap must not be visible.
There must be a small orange rectangle in the middle
It must all be inside the green square
This text must be 0,0,0 black 
The circles must actually be circular, and not a distorted oval",
    tags: "2D, Framebuffer"
)]
    public class FramebufferTest : IRenderable {
        Framebuffer fb;

        public FramebufferTest(FrameworkContext ctx) {
            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "FramebufferTest";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;// 20;

            ctx.SetClearColor(Color.RGBA(1, 1, 1, 1));

            fb = new Framebuffer(1, 1);
            fb.Resize(800, 600);
        }

        public void Render(FrameworkContext ctx) {
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
                ctx.DrawRect(wCX, wCY, wCX + 50, wCY + 25);

            }
            ctx.UseFramebuffer(null);

            ctx.SetDrawColor(1, 0, 0, 1);
            float rectSize = 200;
            ctx.DrawRect(wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);

            ctx.SetTexture(fb.Texture);
            ctx.SetDrawColor(1, 1, 1, 0.5f);
            ctx.DrawRect(0, 0, 800, 600);

            ctx.SetTexture(null);

            ctx.SetDrawColor(0, 1, 0, 0.5f);
            ctx.DrawRectOutline(10, wCX - wCY, wCY - wCY, wCX + wCY, wCY + wCY);
        }

        private void DrawDualCirclesCenter(ref FrameworkContext ctx, float x, float y) {
            ctx.DrawCircle(x - 100, y - 100, 200);
            ctx.DrawCircle(x + 100, y + 100, 200);
        }
    }
}
