using MinimalAF;
using MinimalAF.VisualTests.UI;

namespace RenderingEngineVisualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Element[] tests =
            {
                new UITextInputTest(),
                new UITextNumberInputTest(),
                new UIAspectRatioTest(),
                new UISplittingTest(),
                new UITest(),
                new UILinearArrangeNestedTest(),
                new UILinearArrangeTest(),
            };


            foreach (Element entryPoint in tests)
            {
                new Window(entryPoint).Run();
            }
        }
    }
}
