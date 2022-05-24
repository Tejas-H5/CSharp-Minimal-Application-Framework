using MinimalAF;

namespace RenderingEngineVisualTests {
    class Program {
        static void Main(string[] args) {
            var testRunner = new VisualTestRunner(typeof(FramebufferTest));

            new ApplicationWindow().Run(testRunner);
        }
    }
}
