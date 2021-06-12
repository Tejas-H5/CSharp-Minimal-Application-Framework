using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace MinimalAF.AudioTests
{
    public class MusicAndKeysTest : EntryPoint
    {
        AudioSourceOneShot _clackSound;
        AudioSourceStreamed _streamedSource;
        AudioClipStream _streamProvider;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "music and keyboard test";

            CTX.SetClearColor(0, 0, 0, 0);
            CTX.SetCurrentFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(true, false, clip);

            //music
            AudioData music = AudioData.FromFile("./Res/testMusicShort.mp3");
            _streamProvider = new AudioClipStream(music);

            _streamedSource = new AudioSourceStreamed(true, _streamProvider);
        }


        string oldString = "";

        string KeysToString(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (Input.IsShiftDown)
                {
                    c = char.ToUpper(c);
                }

                sb.Append(CharKeyMapping.CharToString(c));
            }

            return sb.ToString();
        }

        public override void Render(double deltaTime)
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            CTX.DrawText("Press some keys:", Window.Width / 2, Window.Height / 2 + 200);

            string newString = KeysToString(Input.CharactersDown);
            if (newString != oldString)
            {
                oldString = newString;
                _clackSound.Play();
            }

            CTX.DrawText(newString, Window.Width / 2, Window.Height / 2);

            //music
            CTX.SetDrawColor(1, 1, 1, 1);

            string message = "Spacebar to Pause\n";
            if (_streamedSource.GetSourceState() != AudioSourceState.Playing)
            {
                message = "Spacebar to Play\n";
            }

            message += "Time: " + _streamedSource.GetPlaybackPosition();

            CTX.DrawTextAligned(message, Window.Width / 2, Window.Height / 2, HorizontalAlignment.Center, VerticalAlignment.Center);

            CTX.SetDrawColor(1, 0, 0, 1);
            float amount = (float)(_streamedSource.GetPlaybackPosition() / _streamProvider.Duration);
            float x = amount * Window.Width;
            CTX.DrawLine(x, 0, x, Window.Height, 2, CapType.None);

            if (amount > 1)
            {
                CTX.DrawText("Duration: " + _streamProvider.Duration, 0, 0);
            }
        }

        public override void Update(double deltaTime)
        {
            if (Input.IsAnyKeyPressed)
            {
                Console.WriteLine("Pressed: " + Input.CharactersPressed);
            }

            if (Input.IsAnyKeyReleased)
            {
                Console.WriteLine("Released: " + Input.CharactersReleased);
            }

            //music
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

            if (Input.MouseWheelNotches != 0)
            {
                _streamedSource.SetPlaybackPosition(_streamedSource.GetPlaybackPosition() - Input.MouseWheelNotches * 0.5);
            }
        }
    }
}
