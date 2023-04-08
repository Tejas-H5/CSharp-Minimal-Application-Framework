using MinimalAF;
using MinimalAF.Testing;
using AudioEngineTests.AudioTests;

namespace AudioEngine {
    class Program {
        static void Main(string[] args) {
            var tests = new VisualTestRunner();

            tests.AddTest("Music playing", () => new MusicPlayingTest());
            tests.AddTest("First person game", () => new FirstPersonGameTest());
            tests.AddTest("Wav playing (Complicated)", () => new ComplicatedWavPLayingTest());

            new ProgramWindow((ctx) => tests.Init(ctx)).Run();
        }
    }
}
