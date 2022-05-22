using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    [VisualTest(
        description: @"Test that music can be loaded, played and seeked/scrolled through.
Note that this code is copypasted, and the code for " + nameof(MusicAndKeysTest) + " is more up to date",
        tags: "audio"
    )]
    class MusicPlayingTest : Element {
        AudioSourceStreamed streamedSource;
        AudioClipStream streamProvider;

        public override void OnMount(Window w) {
            AudioData music = AudioData.FromFile("./Res/testMusicShort.mp3");
            streamProvider = new AudioClipStream(music);

            streamedSource = new AudioSourceStreamed(true, streamProvider);
        }

        public override void OnRender() {
            SetDrawColor(1, 1, 1, 1);

            string message = "Spacebar to Pause\n\nMousewheel to  move";
            if (streamedSource.GetSourceState() != AudioSourceState.Playing) {
                message = "Spacebar to Play\n\nMousewheel to  move";
            }

            message += "Time: " + streamedSource.GetPlaybackPosition();

            DrawText(message, Width / 2, Height / 2, HAlign.Center, VAlign.Center);

            SetDrawColor(1, 0, 0, 1);
            float amount = (float)(streamedSource.GetPlaybackPosition() / streamProvider.Duration);
            float x = amount * Width;
            DrawLine(x, 0, x, Height, 2, CapType.None);

            if (amount > 1) {
                DrawText("Duration: " + streamProvider.Duration, 0, 0);
            }
        }

        public override void OnUpdate() {
            streamedSource.UpdateStream();

            if (KeyPressed(KeyCode.Space)) {
                if (streamedSource.GetSourceState() != AudioSourceState.Playing) {
                    streamedSource.Play();
                } else {
                    streamedSource.Pause();
                }
            }

            if (MousewheelNotches != 0) {
                streamedSource.SetPlaybackPosition(streamedSource.GetPlaybackPosition() - MousewheelNotches * 0.5);
            }
        }
    }
}
