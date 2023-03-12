using MinimalAF;

namespace RenderingEngineVisualTests {
    class BasicTest : IRenderable {
        public BasicTest(FrameworkContext ctx) {
            if (ctx.Window != null) {
                ctx.SetClearColor(Color.White);
            }
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(Color.Red);
            ctx.DrawRect(0, 0, 1000, 100);
        }
    }


    class Program {
        static void Main(string[] args) {
            // var testRunner = new VisualTestRunner(typeof(FramebufferTest));

            new OpenTKWindowWrapper((ctx) => new NestingTest(ctx))
                .Run();
        }
    }
}
