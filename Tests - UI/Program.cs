using MinimalAF;
using MinimalAF.VisualTests.UI;

namespace RenderingEngineVisualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new UITextInputTest(),
                new UITextNumberInputTest(),
                new UIAspectRatioTest(),
                new UISplittingTest(),
                new UIFitChildrenTest(),
                new UITest(),
                new UILinearArrangeNestedTest(),
                new UILinearArrangeTest(),
                new UIEdgeSnapTest(),
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
