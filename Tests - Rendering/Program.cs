using MinimalAF;
using MinimalAF.VisualTests.Rendering;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            new ApplicationWindow().Run(new VisualTestRunner(typeof(PerspectiveCameraTest)));
        }
    }
}
