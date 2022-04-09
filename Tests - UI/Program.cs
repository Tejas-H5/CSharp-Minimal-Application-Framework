using MinimalAF;
using MinimalAF.VisualTests.UI;

namespace RenderingEngineVisualTests {
    class Program {
        static void Main(string[] args) {
            Element[] tests =
            {
                new UIGoldenRatioTest(),
                new UIResizingTest(),
                new UILinearArrangeTest(),
                new UILinearArrangeNestedTest(),
                new UISplittingTest(),
                new UITextInputTest(),
                new UIAspectRatioTest(),
            };

            MinimalAFEnvironment.Debug = true;
            foreach (Element entryPoint in tests) {
                new Window().Run(entryPoint);
            }
        }
    }
}
