using AudioEngineTests.AudioTests;
using MinimalAF.AudioTests;
using MinimalAF.Logic;

namespace AudioEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new MusicPlayingTest(),
                new BasicWavPlayingTest(),
                new PanningAndListenerDefaultsTest(),
                new PanningTest2(),
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
