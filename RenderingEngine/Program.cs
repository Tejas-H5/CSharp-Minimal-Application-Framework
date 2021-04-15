using RenderingEngine.Logic;
using RenderingEngine.VisualTests;
using RenderingEngine.VisualTests.UI;
using RenderingEngine.VisualTests.UIEditor;

namespace RenderingEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new UIEditor(),
               /* 
                new UITextNumberInputTest(),
                new UITextInputTest(),
                new UITest(),
                new StencilTest(),
                new TextureTest(),
                new KeyboardTest(),
                new Benchmark(5),
                new PolylineTest(),
                new GeometryOutlineTest(),
                new GeometryAndTextTest(),
                new ArcTest(),
                new TextTest(),*/
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
