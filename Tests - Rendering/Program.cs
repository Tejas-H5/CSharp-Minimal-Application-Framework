using MinimalAF.Logic;
using MinimalAF.VisualTests.Rendering;
using RenderingEngineRenderingTests.VisualTests.Rendering;

namespace RenderingEngineVisualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new FramebufferTest(),
                //new StencilTest(),
                //new TextureTest(),
                //new PolylineSelfIntersectionAlgorithmTest(),
                //new PolylineSelfIntersectionAlgorithmTest(),
                //new PolylineTest(),
                //new KeyboardTest(),
                //new Benchmark(5),
                //new GeometryOutlineTest(),
                //new GeometryAndTextTest(),
                //new ArcTest(),
                //new TextTest(),
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
