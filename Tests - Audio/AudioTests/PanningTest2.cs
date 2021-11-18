using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using OpenTK.Mathematics;

namespace MinimalAF.AudioTests
{
    public class PanningTest2 : Element
    {
        AudioSourceOneShot _clackSound;
        AudioListener _listener;

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Panning test 2";

            CTX.SetClearColor(1, 1, 1, 1);
            CTX.Text.SetFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(false, false, clip);

            _listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;
        float prevListenerX, prevListenerZ;

        public override void OnUpdate()
        {
            timer += Time.DeltaTime;
            if (timer > 0.5f)
            {
                timer = 0;
                _clackSound.Play();
            }

            prevListenerX = listenerX;
            prevListenerZ = listenerZ;

            listenerX = 10 * ((Input.Mouse.X / Width) - 0.5f);
            listenerZ = 10 * ((Input.Mouse.Y / Height) - 0.5f);

            _clackSound.Position = new Vector3(listenerX, 0, listenerZ);
            _clackSound.Velocity = new Vector3((listenerX - prevListenerX) / Time.DeltaTime, 0, (listenerZ - prevListenerZ) / Time.DeltaTime);
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.Circle.Draw(Width / 2, Height / 2, 20);


            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.Circle.Draw(Input.Mouse.X, Input.Mouse.Y, 20);
            CTX.DrawTextAligned("source is here (" + listenerX + "," + listenerZ + ")", Input.Mouse.X, Input.Mouse.Y, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
    }
}
