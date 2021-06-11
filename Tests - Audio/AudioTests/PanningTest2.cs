using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using OpenTK.Mathematics;

namespace MinimalAF.AudioTests
{
    public class PanningTest2 : EntryPoint
    {
        AudioSourceOneShot _clackSound;
        AudioListener _listener;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Panning test 2";

            CTX.SetClearColor(1, 1, 1, 1);
            CTX.SetCurrentFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(false, false, clip);

            _listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;
        float prevListenerX, prevListenerZ;

        public override void Update(double deltaTime)
        {
            timer += deltaTime;
            if (timer > 0.5f)
            {
                timer = 0;
                _clackSound.Play();
            }

            prevListenerX = listenerX;
            prevListenerZ = listenerZ;

            listenerX = 10 * ((Input.MouseX / Window.Width) - 0.5f);
            listenerZ = 10 * ((Input.MouseY / Window.Height) - 0.5f);

            _clackSound.Position = new Vector3(listenerX, 0, listenerZ);
            _clackSound.Velocity = new Vector3((listenerX - prevListenerX) / (float)deltaTime, 0, (listenerZ - prevListenerZ) / (float)deltaTime);
        }

        public override void Render(double deltaTime)
        {
            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.DrawCircle(Window.Width / 2, Window.Height / 2, 20);


            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.DrawCircle(Input.MouseX, Input.MouseY, 20);
            CTX.DrawTextAligned("source is here (" + listenerX + "," + listenerZ + ")", Input.MouseX, Input.MouseY, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
    }
}
