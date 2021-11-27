using MinimalAF.Audio;
using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace MinimalAF.AudioTests
{
    public class BasicWavPlayingTest : Element
    {
        AudioSourceOneShot _clackSound;

        public override void OnStart()
        {
            WindowElement w = GetAncestor<WindowElement>();
            w.Size = (800, 600);
            w.Title = "Keyboard test";

            CTX.SetClearColor(0, 0, 0, 0);
            CTX.Text.SetFont("Consolas", 36);

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

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            CTX.Text.Draw("Press some keys:", Width / 2, Height / 2 + 200);

            string newString = KeysToString(Input.CharactersDown);
            if (newString != oldString)
            {
                oldString = newString;
                _clackSound.Play();
            }

            CTX.Text.Draw(newString, Width / 2, Height / 2);
        }

        public override void OnUpdate()
        {
            if (Input.IsAnyKeyPressed)
            {
                Console.WriteLine("Pressed: " + Input.Keyboard.CharactersPressed);
            }

            if (Input.IsAnyKeyReleased)
            {
                Console.WriteLine("Released: " + Input.Keyboard.CharactersReleased);
            }
        }
    }
}
