using AudioEngineTests.AudioTests;
using MinimalAF;
using MinimalAF.AudioTests;

namespace AudioEngine {
    class Program {
        static void Main(string[] args) {
            new ApplicationWindow().Run(new VisualTestRunner());
        }
    }
}
