using MinimalAF;
using MinimalAF.Testing;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests {
    class ErrorTest : IRenderable {
        public void Render(AFContext ctx) {
            throw new Exception("Using this component to test error boundaries");
        }
    }

    // You never know when this will come in handy
    class BasicTest : IRenderable {
        public void Render(AFContext ctx) {
            ctx.SetDrawColor(Color.Red);
            IM.DrawRect(ctx, new Rect(250, 200, 500, 500));
        }
    }


    class Program {
        static void Main(string[] args) {
            var tests = new VisualTestRunner();

            MutableUT8String.Test();

            tests.AddTest("Text stress test", () => new TextStressTest());
            tests.AddTest("Text Test", () => new TextTest());
            tests.AddTest("Text Font Atlas Text", () => new TextFontAtlasText());
            tests.AddTest("Basic test", () => new BasicTest());
            // For now, we are allowing errors to flow into the debugger (not catching them)
            // tests.AddTest("Error test", () => new ErrorTest());
            tests.AddTest("Arc test", () => new ArcTest());
            tests.AddTest("Performance benchmark test", () => new Benchmark());
            tests.AddTest("Framebuffer test", () => new FramebufferTest());
            tests.AddTest("Geometry and text test", () => new GeometryAndTextTest());
            tests.AddTest("Geometry Outline Test", () => new GeometryOutlineTest());
            tests.AddTest("Keyboard Test", () => new KeyboardTest());
            tests.AddTest("MeshOutput Caching Test", () => new MeshOutputCachingTest());
            tests.AddTest("Nesting Test", () => new NestingTest());
            tests.AddTest("Polyline Test", () => new PolylineTest());
            tests.AddTest("Stencil Test", () => new StencilTest());
            tests.AddTest("Texture Test", () => new TextureTest());
            tests.AddTest("Perspective Camera Test", () => new PerspectiveCameraTest());

            new ProgramWindow((ctx) => tests.Init(ctx)).Run();
            //new ProgramWindow((ctx) => new BasicTest()).Run();

            // This is a useful test, but I haven't figured out how to put it with the other tests
            // just yet, because it changes the render frequency, meaning that tests can have
            // an influence on the outcome of other tests.

            //var fpsLimitedTests = new VisualTestRunner();
            //fpsLimitedTests.AddTest("Deltatime powersaving loop + timing test", () => new DeltatimeTest());
            //new ProgramWindow((ctx) => {
            //    ctx.Window.RenderFrequency = 60.0;
            //    return fpsLimitedTests.Init(ctx);
            //}).Run();

            Console.WriteLine("Done");
        }
    }
}
