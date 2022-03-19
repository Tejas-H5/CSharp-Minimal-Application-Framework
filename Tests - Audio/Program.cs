using AudioEngineTests.AudioTests;
using MinimalAF;
using MinimalAF.AudioTests;

namespace AudioEngine {
    class Program {
        static void Main(string[] args) {
            Element[] tests =
            {
                new MusicAndKeysTest(),
                new MusicPlayingTest(),
                new BasicWavPlayingTest(),
                new PanningAndListenerDefaultsTest(),
                new PanningTest2(),
            };


            foreach (Element entryPoint in tests) {
                new Window(entryPoint).Run();
            }
        }
    }
}
