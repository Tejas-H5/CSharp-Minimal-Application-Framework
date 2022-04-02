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
				new NestingTest(),
                new StencilTest(),
                new TextureTest(),
                new GeometryAndTextTest(),
                new TextTest(),
                new PolylineSelfIntersectionAlgorithmTest(),
                new ArcTest(),
                new FramebufferTest(),
                new KeyboardTest(),
                new Benchmark(5),
                new GeometryOutlineTest(),
            };


            var window = new Window();

            foreach (Element entryPoint in tests)
            {
                window.Run(entryPoint);
            }
        }
    }
}
