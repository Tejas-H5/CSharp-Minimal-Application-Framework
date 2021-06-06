using AudioEngineTests.AudioTests;
using RenderingEngine.AudioTests;
using RenderingEngine.Logic;
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
