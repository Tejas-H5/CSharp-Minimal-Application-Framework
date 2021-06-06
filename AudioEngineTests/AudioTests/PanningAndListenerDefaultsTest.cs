using RenderingEngine.Audio.Core;
using AudioEngineTests.AudioTests;
using System;
using System.Threading;

namespace RenderingEngine.AudioTests
{
    public class PanningAndListenerDefaultsTest : AudioTest
    {
        public override void Test()
        {
            AudioCTX.Init();

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            AudioSourceOneShot source = new AudioSourceOneShot(false, false, clip);

            //moved to the left
            AudioListener left = new AudioListener()
                .SetPosition(10, 0, 0);

            //moved to the right
            AudioListener right = new AudioListener()
                .SetPosition(-10, 0, 0);

            AudioListener center = new AudioListener()
                .SetPosition(0, 0, 0);


            AudioListener front = new AudioListener()
                .SetPosition(0, -10, 0);

            //moved to the right
            AudioListener back = new AudioListener()
                .SetPosition(0, 10, 0);

            AudioCTX.SetCurrentListener(left);
            Console.WriteLine("Playing audio from the left...");
            PlaySound(clip, source);

            AudioCTX.SetCurrentListener(right);
            Console.WriteLine("Playing audio from the right...");
            PlaySound(clip, source);

            AudioCTX.SetCurrentListener(center);
            Console.WriteLine("Playing audio from the center...");
            PlaySound(clip, source);

            AudioCTX.Cleanup();
        }

        private static void PlaySound(AudioClipOneShot clip, AudioSourceOneShot source)
        {
            int passedTime = 0;
            while(passedTime < 2000)
            {
                source.Play();

                int clipLen = (int)(clip.Data.Duration * 1000);
                Thread.Sleep(clipLen);

                passedTime += clipLen;
            }
        }
    }
}
