using MinimalAF.Audio.Core;
using AudioEngineTests.AudioTests;
using System;
using System.Threading;

namespace MinimalAF.AudioTests
{
    public class PanningTest2 : AudioTest
    {
        public override void Test()
        {
            AudioCTX.Init();

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            AudioSourceOneShot source = new AudioSourceOneShot(true, false, clip);

            ConsoleKeyInfo k;

            float angle = 0;
            while (angle < MathF.PI * 4)
            {
                float xPos = MathF.Sin(angle);
                float forwardPos = MathF.Cos(angle);
                source.SetPosition(xPos, 0, forwardPos);
                PlaySound(clip, source);
                angle += 0.1f;
            }

            AudioCTX.Cleanup();
        }


        private static void PlaySound(AudioClipOneShot clip, AudioSourceOneShot source)
        {
            int clipLen = (int)(clip.Data.Duration * 1000);
            source.Play();
            Thread.Sleep(clipLen);
        }
    }
}
