using MinimalAF.Audio;

namespace MinimalAF.AudioTests {
    public class PanningAndListenerDefaultsTest : Element {
        AudioSourceOneShot _clackSound;
        AudioListener _listener;

        public override void OnMount(Window w) {
            
            w.Size = (800, 600);
            w.Title = "PanningAndListenerDefaultsTest test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
            SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(false, false, clip);

            _listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;

        public override void OnUpdate() {
            timer += Time.DeltaTime;
            if (timer > 0.5f) {
                timer = 0;
                _clackSound.Play();
            }

            listenerX = 10 * ((MouseX / Width) - 0.5f);
            listenerZ = 10 * ((MouseY / Height) - 0.5f);

            _listener.SetPosition(listenerX, 0, listenerZ);
        }

        public override void OnRender() {
            SetDrawColor(0, 0, 0, 1);
            Circle(Width / 2, Height / 2, 20);


            SetDrawColor(1, 0, 0, 1);
            Circle(MouseX, MouseY, 20);
            Text("You are here (" + listenerX + "," + listenerZ + ")", MouseX, MouseY, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
    }
}
