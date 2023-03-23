using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    // @"Test that music can be loaded, played and seeked/scrolled through.
    // Note that this code is copypasted, and the code for " + nameof(MusicAndKeysTest) + " is more up to date",
    class MusicPlayingTest : IRenderable {
        AudioSourceStreamed streamedSource;
        AudioClipStream streamProvider;

        public MusicPlayingTest() {
            AudioData music = AudioData.FromFile("./Res/testMusicShort.mp3");
            streamProvider = new AudioClipStream(music);

            streamedSource = new AudioSourceStreamed(true, streamProvider);
        }

        public void Render(FrameworkContext ctx) {
            // draw playback
            {
                ctx.SetDrawColor(1, 1, 1, 1);

                string message = "Spacebar to Pause\nMousewheel to  move";
                if (streamedSource.GetSourceState() != AudioSourceState.Playing) {
                    message = "Spacebar to Play\nMousewheel to  move";
                }

                message += "Time: " + streamedSource.GetPlaybackPosition();

                _font.Draw(ctx, message, ctx.VW / 2, ctx.VH / 2, HAlign.Center, VAlign.Center);

                ctx.SetDrawColor(1, 0, 0, 1);
                float amount = (float)(streamedSource.GetPlaybackPosition() / streamProvider.Duration);
                float x = amount * ctx.VW;
                IM.DrawLine(ctx, x, 0, x, ctx.VH, 2, CapType.None);

                if (amount > 1) {
                    _font.Draw(ctx, "Duration: " + streamProvider.Duration, 0, 0);
                }
            }

            // update streams, handle input 
            {
                streamedSource.UpdateStream();

                if (ctx.KeyJustPressed(KeyCode.Space)) {
                    if (streamedSource.GetSourceState() != AudioSourceState.Playing) {
                        streamedSource.Play();
                    } else {
                        streamedSource.Pause();
                    }
                }

                if (ctx.MouseWheelNotches != 0) {
                    streamedSource.SetPlaybackPosition(streamedSource.GetPlaybackPosition() - ctx.MouseWheelNotches * 0.5);
                }
            }
        }
    }
}
