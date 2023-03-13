using System;
using System.Collections.Generic;
using System.Text;
using MinimalAF;
using RenderingEngineVisualTests;

namespace RenderingEngineVisualTests {
    public class NestingTest : IRenderable {
        TextTest textTest;
        PolylineTest polylineTest;
        PerspectiveCameraTest perspectiveCameraTest;
        StencilTest stencilTest;

        public NestingTest() {
            textTest = new TextTest();
            polylineTest = new PolylineTest();
            perspectiveCameraTest = new PerspectiveCameraTest();
            stencilTest = new StencilTest();
        }

        public void Render(FrameworkContext ctx) {
            textTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot: 0)
                    .Height(ctx.VH * 0.5f, pivot: 1)
                    .Inset(10).Use()
            );

            polylineTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot: 1)
                    .Height(ctx.VH * 0.5f, pivot: 1)
                    .Inset(10).Use()
            );

            perspectiveCameraTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot: 0)
                    .Height(ctx.VH * 0.5f, pivot: 0)
                    .Inset(10).Use()
            );

            stencilTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot: 1)
                    .Height(ctx.VH * 0.5f, pivot: 0)
                    .Inset(10).Use()
            );
        }
    }
}
