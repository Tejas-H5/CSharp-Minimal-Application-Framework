using RenderingEngine.Logic;
using RenderingEngine.VisualTests;
using RenderingEngine.VisualTests.UI;
using System;

namespace RenderingEngineVisualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new UIEdgeSnapTest(),
                new UILinearArrangeTest(),
                new UITest(),
                new UITextInputTest(),
                new UITextNumberInputTest()
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
