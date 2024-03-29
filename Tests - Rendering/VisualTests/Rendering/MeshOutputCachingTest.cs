﻿using MinimalAF;
using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    public class MeshOutputCachingTest : IRenderable {
        Mesh<Vertex> mesh;

        public MeshOutputCachingTest() {
            mesh = new Mesh<Vertex>(0, 0, stream:false, allowResizing: true);
            IM.DrawCircle(mesh, 0, 0, 120f);
        }

        public void Render(AFContext ctx) {
            ctx.SetDrawColor(1, 0, 0, 0.4f);

            for (int i = 0; i < 10; i++) {
                float xNorm = i / 9f;

                float x = ctx.VW * xNorm;
                float y = ctx.VH * xNorm;

                ctx.SetDrawColor(xNorm, 0, 1f - xNorm, 0.4f);
                ctx.SetTransform(Matrix4.CreateTranslation(x, y, 0));
                mesh.Render();
            }
        }
    }
}
