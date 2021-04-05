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
                new PolylineTest(),
                new GeometryOutlineTest(),
                new StencilTest(),
                new UITest(),
                new Benchmark(5),
                new GeometryAndTextTest(),
                new TextureTest(),
                new ArcTest(),
                new TextTest(),
            };


            foreach(EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
