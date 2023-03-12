using MinimalAF;
using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Testing mesh creation using the immediate mode interface, and then drawing that mesh later.
Would be pretty cool if this worked.",
        tags: "2D, mesh"
    )]
    public class MeshOutputCachingTest : IRenderable {
        Mesh<Vertex> mesh;

        public MeshOutputCachingTest() {
            var data = new MeshData<Vertex>();

            var imRenderer = new ImmediateMode2DDrawer<Vertex>(data);

            imRenderer.Circle.Draw(0, 0, 120f);

            mesh = data.ToDrawableMesh();
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(1, 0, 0, 0.4f);

            for (int i = 0; i < 10; i++) {
                float xNorm = i / 9f;

                float x = ctx.VW * xNorm;
                float y = ctx.VH * xNorm;

                ctx.SetDrawColor(xNorm, 0, 1f - xNorm, 0.4f);
                ctx.SetTransform(Matrix4.CreateTranslation(x, y, 0));
                mesh.Draw();
            }
        }
    }
}
