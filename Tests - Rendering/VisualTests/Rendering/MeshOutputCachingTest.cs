using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Testing mesh creation using the immediate mode interface, and then drawing that mesh later.
Would be pretty cool if this worked.",
        tags: "2D, mesh"
    )]
    public class MeshOutputCachingTest : Element {
        Mesh<Vertex> mesh;

        public MeshOutputCachingTest() {
            var data = new MeshData<Vertex>();

            var imRenderer = new ImmediateMode2DDrawer<Vertex>(data);

            imRenderer.Circle.Draw(0, 0, 120f);

            mesh = data.ToDrawableMesh();
        }

        public override void OnRender() {
            SetDrawColor(1, 0, 0, 0.4f);

            for (int i = 0; i < 10; i++) {
                float xNorm = i / 9f;

                float x = VW(xNorm);
                float y = VH(xNorm);

                SetDrawColor(xNorm, 0, 1f - xNorm, 0.4f);
                SetTransform(Translation(x, y, 0));
                mesh.Draw();
            }
        }
    }
}
