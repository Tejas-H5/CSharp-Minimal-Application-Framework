using MinimalAF;
using MinimalAF.VisualTests.UI;

namespace RenderingEngineVisualTests {
    class Program {
        static void Main(string[] args) {
            Element[] tests =
            {
                new UIResizingTest(),
                new UILinearArrangeTest(),
                new UILinearArrangeNestedTest(),
                new UITest(),
                new UISplittingTest(),
                new UITextInputTest(),
                new UIAspectRatioTest(),
            };


            foreach (Element entryPoint in tests) {
                new Window().Run(entryPoint);
            }
        }
    }
}
