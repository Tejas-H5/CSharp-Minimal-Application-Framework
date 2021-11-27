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
                new UITest(),
                new UISplittingTest(),
                new UITextInputTest(),
                new UITextNumberInputTest(),
                new UIAspectRatioTest(),
                new UILinearArrangeNestedTest(),
                new UILinearArrangeTest(),
            };


            foreach (Element entryPoint in tests)
            {
                new WindowElement(entryPoint).Run();
            }
        }
    }
}
