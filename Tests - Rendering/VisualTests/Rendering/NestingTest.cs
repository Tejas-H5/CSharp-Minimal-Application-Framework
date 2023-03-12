using System;
using System.Collections.Generic;
using System.Text;
using MinimalAF;
using RenderingEngineVisualTests;

namespace RenderingEngineVisualTests
{
	[VisualTest(
        description: @"Test that multiple elements can be rendered side by side without interfering with one another.
This was the one of the main reasons why I am making this in the first place, and not just using Processing",
        tags: "2D, nesting"
    )]
	public class NestingTest : IRenderable {
        TextTest textTest;
        PolylineTest polylineTest;
        PerspectiveCameraTest perspectiveCameraTest;
        StencilTest stencilTest;

        public NestingTest(FrameworkContext ctx)
		{
            textTest = new TextTest(new FrameworkContext{});
            polylineTest = new PolylineTest(new FrameworkContext{});
            perspectiveCameraTest = new PerspectiveCameraTest(new FrameworkContext{});
            stencilTest = new StencilTest(new FrameworkContext{});

            if (ctx.Window == null) return;

            ctx.SetClearColor(Color.White);
            ctx.Window.Title = "Nested rendering test (1 level deep)";
		}

        public void Render(FrameworkContext ctx) {
            textTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot:0)
                    .Height(ctx.VH * 0.5f, pivot:1)
                    .Inset(10).Use()
            );

            polylineTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot:1)
                    .Height(ctx.VH * 0.5f, pivot:1)
                    .Inset(10).Use()
            );

            perspectiveCameraTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot:0)
                    .Height(ctx.VH * 0.5f, pivot:0)
                    .Inset(10).Use()
            );

            stencilTest.Render(
                ctx.Width(ctx.VW * 0.5f, pivot:1)
                    .Height(ctx.VH * 0.5f, pivot:0)
                    .Inset(10).Use()
            );
        }
    }
}
        