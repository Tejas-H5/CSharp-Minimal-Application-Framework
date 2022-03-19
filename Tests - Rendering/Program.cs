using MinimalAF;
using MinimalAF.VisualTests.Rendering;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            Element[] tests =
            {
                new PolylineTest(),
                new TextureTest(),
                new GeometryAndTextTest(),
                new StencilTest(),
				new NestingTest(),
                new TextTest(),
                new PolylineSelfIntersectionAlgorithmTest(),
                new ArcTest(),
                new FramebufferTest(),
                new KeyboardTest(),
                new Benchmark(5),
                new GeometryOutlineTest(),
            };


            foreach (Element entryPoint in tests)
            {
                new Window(entryPoint).Run();
            }
        }
    }
}
