using AudioEngineTests.AudioTests;
using MinimalAF.AudioTests;
using MinimalAF;

namespace AudioEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Element[] tests =
            {
                new MusicAndKeysTest(),
                new MusicPlayingTest(),
                new BasicWavPlayingTest(),
                new PanningAndListenerDefaultsTest(),
                new PanningTest2(),
            };


            foreach (Element entryPoint in tests)
            {
                new WindowElement(entryPoint).Run();
            }
        }
    }
}
