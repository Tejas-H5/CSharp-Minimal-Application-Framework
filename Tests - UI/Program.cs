using MinimalAF;

namespace UIVisualTests {
    class Program {
        static void Main(string[] args) {
            new ApplicationWindow().Run(new VisualTestRunner(typeof(UIContentSizeFittingTest)));
        }
    }
}
