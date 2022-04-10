using MinimalAF;
using MinimalAF.Testing;
using MinimalAF.VisualTests.Rendering;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            VisualTestRunner runner = new VisualTestRunner();
            var window = new Window();

            window.Run(runner);
        }
    }
}
