using MinimalAF.Audio;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace MinimalAF.AudioTests {
	[VisualTest]
    public class MusicAndKeysTest : Element {
        AudioSourceOneShot _clackSound;
        AudioSourceStreamed _streamedSource;
        AudioClipStream _streamProvider;

        public override void OnMount(Window w) {
            
            w.Size = (800, 600);
            w.Title = "music and keyboard test";

            SetClearColor(Color4.RGBA(0, 0, 0, 0));
            SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(true, false, clip);

            //music
            AudioData music = AudioData.FromFile("./Res/testMusicShort.mp3");
            _streamProvider = new AudioClipStream(music);

            _streamedSource = new AudioSourceStreamed(true, _streamProvider);
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

            Text("Press some keys:", Width / 2, Height / 2 + 200, HorizontalAlignment.Center, VerticalAlignment.Center);

            string newString = KeysToString(KeyboardCharactersHeld);
            if (newString != oldString) {
                oldString = newString;
                _clackSound.Play();
            }

            Text(newString, Width / 2, Height / 2, HorizontalAlignment.Center, VerticalAlignment.Center);

            //music
            SetDrawColor(1, 1, 1, 1);

            string message = "Spacebar to Pause\n";
            if (_streamedSource.GetSourceState() != AudioSourceState.Playing) {
                message = "Spacebar to Play\nMousewheel to  move";
            }

            message += "Time: " + _streamedSource.GetPlaybackPosition();

            Text(message, Width / 2, Height / 2, HorizontalAlignment.Center, VerticalAlignment.Center);

            SetDrawColor(1, 0, 0, 1);
            float amount = (float)(_streamedSource.GetPlaybackPosition() / _streamProvider.Duration);
            float x = amount * Width;
            Line(x, 0, x, Height, 2, CapType.None);

            if (amount > 1) {
                Text("Duration: " + _streamProvider.Duration, 0, 0);
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
            _streamedSource.UpdateStream();

            if (KeyPressed(KeyCode.Space)) {
                if (_streamedSource.GetSourceState() != AudioSourceState.Playing) {
                    _streamedSource.Play();
                } else {
                    _streamedSource.Pause();
                }
            }

            if (MouseWheelNotches != 0) {
                _streamedSource.SetPlaybackPosition(_streamedSource.GetPlaybackPosition() - MouseWheelNotches * 0.5);
            }
        }
    }
}
