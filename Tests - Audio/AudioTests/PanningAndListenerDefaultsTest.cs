using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    [VisualTest(
        description: @"Test the audio listner positioning abilities",
        tags: "audio"
    )]
    public class PanningAndListenerDefaultsTest : Element {
        AudioSourceOneShot clackSound;
        AudioListener listener;

        public override void OnMount() {
            
            w.Size = (800, 600);
            w.Title = "PanningAndListenerDefaultsTest test";

            SetClearColor(Color.RGBA(1, 1, 1, 1));
            SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(false, false, clip);

            listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;

        public override void OnUpdate() {
            timer += Time.DeltaTime;
            if (timer > 0.5f) {
                timer = 0;
                clackSound.Play();
            }

            listenerX = 10 * ((MouseX / Width) - 0.5f);
            listenerZ = 10 * ((MouseY / Height) - 0.5f);

            listener.SetPosition(listenerX, 0, listenerZ);
        }

        public override void OnRender() {
            ctx.SetDrawColor(0, 0, 0, 1);
            DrawCircle(Width / 2, Height / 2, 20);


            ctx.SetDrawColor(1, 0, 0, 1);
            DrawCircle(MouseX, MouseY, 20);
            DrawText("You are here (" + listenerX + "," + listenerZ + ")", MouseX, MouseY, HAlign.Center, VAlign.Bottom);
        }
    }
}
