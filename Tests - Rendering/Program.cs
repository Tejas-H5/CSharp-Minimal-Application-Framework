using MinimalAF;
using MinimalAF.VisualTests.Rendering;
using System;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            //new ApplicationWindow().Run(new VisualTestRunner(typeof(PerspectiveCameraTest)));

            string text = "a b c d";

            var iter = new StringIterator(text, " ");
            var a = iter.GetNext();
            Console.WriteLine("[" + a.ToString() + "]");
            a = iter.GetNext();
            Console.WriteLine("[" + a.ToString() + "]");
            a = iter.GetNext();
            Console.WriteLine("[" + a.ToString() + "]");
            a = iter.GetNext();
            Console.WriteLine("[" + a.ToString() + "]");
        }
    }
}
