using RenderingEngine.Audio.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngineTests.AudioTests
{
    class MusicPlayingTest : AudioTest
    {
        public override void Test()
        {
            AudioCTX.Init();

            AudioData music = AudioData.FromFile("./Res/testMusic.mp3");
            AudioClipStreamed streamProvider = new AudioClipStreamed(music);

            AudioSourceStreamed streamedSource = new AudioSourceStreamed(true, streamProvider);

            streamedSource.Play();

            while (streamedSource.GetSourceState() == AudioSourceState.Playing)
            {
                streamedSource.UpdateStream();
            }

            AudioCTX.Cleanup();
        }
    }
}
