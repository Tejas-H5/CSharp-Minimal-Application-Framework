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
                new UITest(),
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


            foreach(EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
