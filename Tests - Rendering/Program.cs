using MinimalAF;
using MinimalAF.VisualTests.Rendering;
using System;

namespace RenderingEngineVisualTests
{
	class Program
    {
        static void Main(string[] args)
        {
            new ApplicationWindow().Run(new VisualTestRunner(typeof(FramebufferTest)));
        }
    }
}
