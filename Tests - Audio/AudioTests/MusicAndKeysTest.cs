using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    [VisualTest(
        description: @"Test that the music and clips can both play at the same time. 
Right now, this test is failing imo. The one-shot sounds are not audible over the music.",
        tags: "audio"
    )]
    public class MusicAndKeysTest : Element {
        AudioSourceOneShot clackSound;
        AudioSourceStreamed streamedSource;
        AudioClipStream streamProvider;

        public override void OnMount() {
            
            w.Size = (800, 600);
            w.Title = "music and keyboard test";

            SetClearColor(Color.RGBA(0, 0, 0, 0));
            SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(true, false, clip);

            //music
            AudioData music = AudioData.FromFile("./Res/testMusicShort.mp3");
            streamProvider = new AudioClipStream(music);

            streamedSource = new AudioSourceStreamed(true, streamProvider);
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
            ctx.SetDrawColor(1, 1, 1, 1);

            DrawText("Press some keys:", Width / 2, Height / 2 + 200, HAlign.Center, VAlign.Center);

            string newString = KeysToString(KeyboardCharactersHeld);
            if (newString != oldString) {
                oldString = newString;
                clackSound.Play();
            }

            DrawText(newString, Width / 2, Height / 2, HAlign.Center, VAlign.Center);

            //music
            ctx.SetDrawColor(1, 1, 1, 1);

            string message = "Spacebar to Pause\nMousewheel to  move";
            if (streamedSource.GetSourceState() != AudioSourceState.Playing) {
                message = "Spacebar to Play\nMousewheel to  move";
            }

            message += "Time: " + streamedSource.GetPlaybackPosition();

            DrawText(message, Width / 2, Height / 2, HAlign.Center, VAlign.Center);

            ctx.SetDrawColor(1, 0, 0, 1);
            float amount = (float)(streamedSource.GetPlaybackPosition() / streamProvider.Duration);
            float x = amount * Width;
            DrawLine(x, 0, x, Height, 2, CapType.None);

            if (amount > 1) {
                DrawText("Duration: " + streamProvider.Duration, 0, 0);
            }
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Any)) {
                Console.WriteLine("Pressed: " + KeyboardCharactersPressed);
            }

            if (KeyReleased(KeyCode.Any)) {
                Console.WriteLine("Released: " + KeyboardCharactersReleased);
            }

            //music
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
