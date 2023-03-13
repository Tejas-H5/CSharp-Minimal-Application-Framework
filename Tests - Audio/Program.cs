using MinimalAF;
using MinimalAF.Testing;
using AudioEngineTests.AudioTests;

namespace AudioEngine {
    class Program {
        static void Main(string[] args) {
            var tests = new VisualTestRunner();

            tests.AddTest("Wav playing", () => new BasicWavPlayingTest());
            tests.AddTest("Music playing", () => new MusicPlayingTest());
            tests.AddTest("Panning and listner defaults", () => new PanningAndListenerDefaultsTest());
            tests.AddTest("Panning test 2", () => new PanningTest2());

            new ProgramWindow((ctx) => tests.Init(ctx)).Run();
        }
    }
}
