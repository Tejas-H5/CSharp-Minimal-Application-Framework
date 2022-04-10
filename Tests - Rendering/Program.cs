using MinimalAF;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            VisualTestRunner runner = new VisualTestRunner();
            var window = new ApplicationWindow();

            window.Run(runner);
        }
    }
}
