using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using RenderingEngine.VisualTests;
using RenderingEngine.Logic;
using System;

namespace RenderingEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new GeometryAndTextTest(),
                new Benchmark(5),
                new TextureTest(),
                new ArcTest(),
                new TextTest(),
                new TriangleTest(),
                new UITest()
            };


            foreach(EntryPoint entryPoint in tests)
            {
                using (var window = new GraphicsWindow(entryPoint))
                {
                    window.Run();
                }
            }
        }
    }
}
