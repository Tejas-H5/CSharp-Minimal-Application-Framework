using AudioEngineTests.AudioTests;
using MinimalAF.Audio.Core;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace MinimalAF.AudioTests
{
    public class BasicWavPlayingTest : EntryPoint
    {
        AudioSourceOneShot _clackSound;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Keyboard test";

            CTX.SetClearColor(0, 0, 0, 0);
            CTX.SetCurrentFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(true, false, clip);
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
        }
    }
}
