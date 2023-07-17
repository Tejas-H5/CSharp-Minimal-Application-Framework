using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    // @"Test that music can be loaded, played and seeked/scrolled through.
    // Note that this code is copypasted, and the code for " + nameof(MusicAndKeysTest) + " is more up to date",
    class MusicPlayingTest : IRenderable, IDisposable {
        AudioSource _source;
        AudioStreamInput _audioStream;
        AudioStreamRawData _audioClipStream;
        DrawableFont _font = new DrawableFont("Consolas", 16);
        AudioListener _listener;

        public MusicPlayingTest() {
            _audioStream = new AudioStreamInput(
                _audioClipStream = new AudioStreamRawData(
                    AudioRawData.FromFile("./Res/testMusicShort.mp3")
                )
            );

            _source = new AudioSource();

            _source.SetInput(_audioStream);
            _listener = new AudioListener();
        }

        public void Dispose() {
            if (_source == null) return;

            // stop all sounds we are playing
            _source.Stop();
        }

        public void Render(AFContext ctx) {
            {
                string message = "Spacebar to Pause\nMousewheel to  move";
                if (_source.PlaybackState != PlaybackState.Playing) {
                    message = "Spacebar to Play\nMousewheel to  move";
                }

                var playbackPos = _source.PlaybackPosition;
                message += "Time: " + playbackPos;

                _font.DrawText(ctx, message, ctx.VW / 2, ctx.VH / 2, HAlign.Center, VAlign.Center);

                ctx.SetDrawColor(1, 0, 0, 1);
                float amount = (float)(playbackPos / _audioClipStream.Duration);
                float x = amount * ctx.VW;
                IM.DrawLine(ctx, x, 0, x, ctx.VH, 2, CapType.None);

                if (amount > 1) {
                    _font.DrawText(ctx, "Duration: " + _audioClipStream.Duration, 0, 0, HAlign.Left, VAlign.Bottom);
                }
            }

            // update streams, handle input 
            {
                _listener.MakeCurrent();

                if (ctx.KeyJustPressed(KeyCode.Space)) {
                    if (_source.PlaybackState != PlaybackState.Playing) {
                        _source.Play();
                    } else {
                        _source.Pause();
                    }
                }

                if (ctx.MouseWheelNotches != 0) {
                    _source.PlaybackPosition =
                        _source.PlaybackPosition - ctx.MouseWheelNotches * 0.5;
                }
            }
        }
    }
}
