using MinimalAF;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    internal class PerspectiveCameraTest : IRenderable {
        float zPos;
        public PerspectiveCameraTest(float zPos = 10) {
            this.zPos = zPos;
        }

        DrawableFont _font = new DrawableFont("Source code pro", new DrawableFontOptions { });

        public void Render(AFContext ctx) {
            ctx.SetProjectionPerspective((float)Math.PI / 2.0f, 0.1f, 1000);

            Vector3 rectPoint = new Vector3(
                (ctx.MouseX - ctx.VW / 2) / 10f,
                (ctx.MouseY - ctx.VH / 2) / 10f,
                -20f
            );

            ctx.SetViewLookAt(
                position: new Vector3(0, 0, zPos),
                target: rectPoint,
                up: new Vector3(0, 1, 0)
            );

            ctx.SetModel(Matrix4.CreateTranslation(new Vector3(0, 0, -20)));
            ctx.SetDrawColor(0, 0, 0, 1);
            IM.DrawRectOutline(ctx, 1, -50, -50, 50, 50);

            _font.DrawText(ctx, "This is a wall.", 24, new DrawTextOptions { HAlign = 0.5f, VAlign = 0.5f });

            ctx.SetModel(Matrix4.CreateTranslation(rectPoint));

            ctx.SetDrawColor(1, 0, 0, 0.5f);
            IM.DrawRect(ctx, new Rect(-1, -1, 1, 1) * 10);
        }
    }
}
