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
                new MusicPlayingTest(),
                new PanningAndListenerDefaultsTest(),
                new PanningTest2(),
                new PanningWithVelocityTest(),
                //new BasicWavPlayingTest(), //This test is not automatic
            };


            foreach (EntryPoint entryPoint in tests)
            {
                Window.RunProgram(entryPoint);
            }
        }
    }
}
