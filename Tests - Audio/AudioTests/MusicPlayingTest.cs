using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;

namespace AudioEngineTests.AudioTests
{
    class MusicPlayingTest : Element
    {
        AudioSourceStreamed _streamedSource;
        AudioClipStream _streamProvider;

        public override void OnStart()
        {
            AudioData music = AudioData.FromFile("./Res/testMusicShort.mp3");
            _streamProvider = new AudioClipStream(music);

            _streamedSource = new AudioSourceStreamed(true, _streamProvider);
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            string message = "Spacebar to Pause\n";
            if (_streamedSource.GetSourceState() != AudioSourceState.Playing)
            {
                message = "Spacebar to Play\n";
            }

            message += "Time: " + _streamedSource.GetPlaybackPosition();

            CTX.DrawTextAligned(message, Width / 2, Height / 2, HorizontalAlignment.Center, VerticalAlignment.Center);

            CTX.SetDrawColor(1, 0, 0, 1);
            float amount = (float)(_streamedSource.GetPlaybackPosition() / _streamProvider.Duration);
            float x = amount * Width;
            CTX.DrawLine(x, 0, x, Height, 2, CapType.None);

            if(amount > 1)
            {
                CTX.DrawText("Duration: " + _streamProvider.Duration, 0, 0);
            }
        }

        public override void OnUpdate()
        {
            _streamedSource.UpdateStream();

            if (Input.IsKeyPressed(KeyCode.Space))
            {
                if (_streamedSource.GetSourceState() != AudioSourceState.Playing)
                {
                    _streamedSource.Play();
                }
                else
                {
                    _streamedSource.Pause();
                }
            }

            if(Input.MouseWheelNotches != 0)
            {
                _streamedSource.SetPlaybackPosition(_streamedSource.GetPlaybackPosition() - Input.MouseWheelNotches*0.5);
            }
        }
    }
}
