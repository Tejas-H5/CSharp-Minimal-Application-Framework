using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    [VisualTest(
        description: @"Test audio loading and single sound playback",
        tags: "audio"
    )]
    public class BasicWavPlayingTest : Element {
        AudioSourceOneShot clackSound;

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Keyboard test";

            SetClearColor(Color4.RGBA(0, 0, 0, 0));
            SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(true, false, clip);
        }


        string oldString = "";

        string KeysToString(string s) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];
                if (KeyHeld(KeyCode.Shift)) {
                    c = char.ToUpper(c);
                }

                sb.Append(CharKeyMapping.CharToString(c));
            }

            return sb.ToString();
        }

        public override void OnRender() {
            SetDrawColor(1, 1, 1, 1);

            DrawText("Press some keys:", Width / 2, Height / 2 + 200);

            string newString = KeysToString(KeyboardCharactersHeld);
            if (newString != oldString) {
                oldString = newString;
                clackSound.Play();
            }

            DrawText(newString, Width / 2, Height / 2);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Any)) {
                Console.WriteLine("Pressed: " + KeyboardCharactersPressed);
            }

            if (KeyReleased(KeyCode.Any)) {
                Console.WriteLine("Released: " + KeyboardCharactersReleased);
            }
        }
    }
}
