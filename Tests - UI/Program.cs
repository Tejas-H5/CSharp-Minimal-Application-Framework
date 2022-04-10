using MinimalAF;
using MinimalAF.VisualTests.UI;

namespace RenderingEngineVisualTests {
    class Program {
        static void Main(string[] args) {
            Element[] tests =
            {
                new UIDragTest(),
                new UILinearArrangeNestedTest(),
                new UIAspectRatioTest(),
                new UITextInputTest(),
                new UISplittingTest(),
                new UIGoldenRatioTest(),
                new UIResizingTest(),
                new UILinearArrangeTest(),
            };

            MinimalAFEnvironment.Debug = true;
            foreach (Element entryPoint in tests) {
                new ApplicationWindow().Run(entryPoint);
            }
        }
    }
}
