using MinimalAF;
using MinimalAF.Testing;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests {
    class ErrorTest : IRenderable {
        public void Render(FrameworkContext ctx) {
            throw new Exception("Using this component to test error boundaries");
        }
    }


    class Program {
        static void Main(string[] args) {
            var tests = new VisualTestRunner();

            tests.AddTest("Text Test", () => new TextTest());
            tests.AddTest("Error test", () => new ErrorTest());
            tests.AddTest("Arc test", () => new ArcTest());
            tests.AddTest("Performance benchmark test", () => new Benchmark());
            tests.AddTest("Framebuffer test", () => new FramebufferTest());
            tests.AddTest("Geometry and text test", () => new GeometryAndTextTest());
            tests.AddTest("Geometry Outline Test", () => new GeometryOutlineTest());
            tests.AddTest("Keyboard Test", () => new KeyboardTest());
            tests.AddTest("MeshOutput Caching Test", () => new MeshOutputCachingTest());
            tests.AddTest("Nesting Test", () => new NestingTest());
            tests.AddTest("Polyline Self Intersection Algorithm Test", () => new PolylineSelfIntersectionAlgorithmTest());
            tests.AddTest("Polyline Test", () => new PolylineTest());
            tests.AddTest("Stencil Test", () => new StencilTest());
            tests.AddTest("Text Font Atlas Text", () => new TextFontAtlasText());
            tests.AddTest("Texture Test", () => new TextureTest());
            tests.AddTest("Perspective Camera Test", () => new PerspectiveCameraTest());

            new ProgramWindow((ctx) => tests.Init(ctx)).Run();
        }
    }
}
