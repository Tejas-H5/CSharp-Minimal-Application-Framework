using AudioEngineTests.AudioTests;
using RenderingEngine.Audio.Core;
using System;

namespace RenderingEngine.AudioTests
{
    public class BasicWavPlayingTest : AudioTest
    {
        public override void Test()
        {
            AudioCTX.Init();

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            AudioSourceOneShot source = new AudioSourceOneShot(true, false, clip);

            ConsoleKeyInfo k;

            while((k = Console.ReadKey()).KeyChar != 'x')
            {
                source.Play();
            } 

            AudioCTX.Cleanup();
        }
    }
}
