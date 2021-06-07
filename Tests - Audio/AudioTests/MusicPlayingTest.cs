using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngineTests.AudioTests
{
    class MusicPlayingTest : EntryPoint
    {
        AudioSourceStreamed _streamedSource;

        public override void Start()
        {
            AudioData music = AudioData.FromFile("./Res/testMusic.mp3");
            AudioClipStreamed streamProvider = new AudioClipStreamed(music);

            _streamedSource = new AudioSourceStreamed(true, streamProvider);
        }

        public override void Render(double deltaTime)
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            string message = "Spacebar to Pause";
            if(_streamedSource.GetSourceState() != AudioSourceState.Playing){
                message = "Spacebar to Play";
            }

            CTX.DrawTextAligned(message, Window.Width/2, Window.Height/2, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public override void Update(double deltaTime)
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
        }
    }
}
