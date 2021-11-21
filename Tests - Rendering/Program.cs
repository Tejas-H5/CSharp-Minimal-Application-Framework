using MinimalAF;
using MinimalAF.VisualTests.Rendering;
using RenderingEngineRenderingTests.VisualTests.Rendering;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            Element[] tests =
            {
                new PolylineSelfIntersectionAlgorithmTest(),
                new ArcTest(),
                new FramebufferTest(),
                new GeometryAndTextTest(),
                new StencilTest(),
                new TextureTest(),
                new PolylineTest(),
                new KeyboardTest(),
                new Benchmark(5),
                new GeometryOutlineTest(),
                new TextTest(),
            };


            foreach (Element entryPoint in tests)
            {
                new Window(entryPoint).Run();
            }
        }
    }
}
