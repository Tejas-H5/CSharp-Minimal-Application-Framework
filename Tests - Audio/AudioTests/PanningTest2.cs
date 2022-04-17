using MinimalAF.Audio;
using OpenTK.Mathematics;

namespace MinimalAF.AudioTests {
    [VisualTest]
    public class PanningTest2 : Element {
        AudioSourceOneShot clackSound;
        AudioListener listener;

        public override void OnMount(Window w) {
            
            w.Size = (800, 600);
            w.Title = "Panning test 2";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
            SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(false, false, clip);

            listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;
        float prevListenerX, prevListenerZ;

        public override void OnUpdate() {
            timer += Time.DeltaTime;
            if (timer > 0.5f) {
                timer = 0;
                clackSound.Play();
            }

            prevListenerX = listenerX;
            prevListenerZ = listenerZ;

            listenerX = 10 * ((MouseX / Width) - 0.5f);
            listenerZ = 10 * ((MouseY / Height) - 0.5f);

            clackSound.Position = new Vector3(listenerX, 0, listenerZ);
            clackSound.Velocity = new Vector3((listenerX - prevListenerX) / Time.DeltaTime, 0, (listenerZ - prevListenerZ) / Time.DeltaTime);
        }

        public override void OnRender() {
            SetDrawColor(0, 0, 0, 1);
            Circle(Width / 2, Height / 2, 20);


            SetDrawColor(1, 0, 0, 1);
            Circle(MouseX, MouseY, 20);
            Text("source is here (" + listenerX + "," + listenerZ + ")", MouseX, MouseY, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
    }
}
