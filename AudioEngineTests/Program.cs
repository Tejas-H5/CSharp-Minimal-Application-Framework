using AudioEngineTests.AudioTests;
using MinimalAF.AudioTests;
using MinimalAF.Logic;
using System;

namespace AudioEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint[] tests =
            {
                new BasicWavPlayingTest(),
                new MusicPlayingTest(),
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
