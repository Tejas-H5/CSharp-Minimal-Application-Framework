using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;

namespace MinimalAF.AudioTests
{
    public class PanningAndListenerDefaultsTest : Element
    {
        AudioSourceOneShot _clackSound;
        AudioListener _listener;

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "PanningAndListenerDefaultsTest test";

            CTX.SetClearColor(1, 1, 1, 1);
            CTX.SetCurrentFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(false, false, clip);

            _listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;

        public override void OnUpdate()
        {
            timer += Time.DeltaTime;
            if (timer > 0.5f)
            {
                timer = 0;
                _clackSound.Play();
            }

            listenerX = 10 * ((Input.MouseX / Width) - 0.5f);
            listenerZ = 10 * ((Input.MouseY / Height) - 0.5f);

            _listener.SetPosition(listenerX, 0, listenerZ);
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.DrawCircle(Width / 2, Height / 2, 20);


            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.DrawCircle(Input.MouseX, Input.MouseY, 20);
            CTX.DrawTextAligned("You are here (" + listenerX + "," + listenerZ + ")", Input.MouseX, Input.MouseY, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
    }
}
