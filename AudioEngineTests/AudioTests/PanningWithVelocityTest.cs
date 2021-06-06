using AudioEngineTests.AudioTests;
using RenderingEngine.Audio.Core;
using System;
using System.Threading;

namespace RenderingEngine.AudioTests
{
    public class PanningWithVelocityTest : AudioTest
    {
        public override void Test()
        {
            AudioCTX.Init();

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            AudioSourceOneShot source = new AudioSourceOneShot(true, false, clip);

            ConsoleKeyInfo k;

            float angle = 0;
            float lastPosX = MathF.Sin(angle);
            float lastPosZ = MathF.Cos(angle);

            float mult = 10;

            while (angle < MathF.PI * 4)
            {
                float xPos = MathF.Sin(angle);
                float forwardPos = MathF.Cos(angle);

                float velX = xPos - lastPosX;
                float vely = forwardPos - lastPosZ;

                lastPosX = xPos;
                lastPosZ = forwardPos;

                source
                    .SetPosition(xPos, 0, forwardPos)
                    .SetVelocity(mult * velX, mult * vely, mult * 0);

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
