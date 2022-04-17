using MinimalAF;
using MinimalAF.Audio;
using MinimalAF.Rendering;

namespace AudioEngineTests.AudioTests {
	[VisualTest]
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

            string message = "Spacebar to Pause\n";
            if (streamedSource.GetSourceState() != AudioSourceState.Playing) {
                message = "Spacebar to Play\n";
            }

            message += "Time: " + streamedSource.GetPlaybackPosition();

            Text(message, Width / 2, Height / 2, HorizontalAlignment.Center, VerticalAlignment.Center);

            SetDrawColor(1, 0, 0, 1);
            float amount = (float)(streamedSource.GetPlaybackPosition() / streamProvider.Duration);
            float x = amount * Width;
            Line(x, 0, x, Height, 2, CapType.None);

            if (amount > 1) {
                Text("Duration: " + streamProvider.Duration, 0, 0);
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
