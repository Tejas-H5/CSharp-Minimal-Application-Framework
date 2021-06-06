using MinimalAF.Logic;
using MinimalAF.VisualTests.Rendering;
using RenderingEngineRenderingTests.VisualTests.Rendering;
using System;

namespace RenderingEngineVisualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new StencilTest(),
                new TextureTest(),
                new KeyboardTest(),
                new Benchmark(5),
                new PolylineTest(),
                new GeometryOutlineTest(),
                new GeometryAndTextTest(),
                new ArcTest(),
                new TextTest(),
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
