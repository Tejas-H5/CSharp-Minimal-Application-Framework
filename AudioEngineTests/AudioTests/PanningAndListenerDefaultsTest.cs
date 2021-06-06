using MinimalAF.Audio.Core;
using AudioEngineTests.AudioTests;
using System;
using System.Threading;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.Datatypes.UI;

namespace MinimalAF.AudioTests
{
    public class PanningAndListenerDefaultsTest : EntryPoint
    {
        AudioSourceOneShot _clackSound;
        AudioListener _listener;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "PanningAndListenerDefaultsTest test";

            CTX.SetClearColor(1,1,1,1);
            CTX.SetCurrentFont("Consolas", 36);

            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            _clackSound = new AudioSourceOneShot(false, false, clip);

            _listener = new AudioListener();
            AudioCTX.SetCurrentListener(_listener);
        }

        double timer = 0;
        float listenerX, listenerZ;

        public override void Update(double deltaTime)
        {
            timer += deltaTime;
            if(timer > 0.5f)
            {
                timer = 0;
                _clackSound.Play();
            }

            listenerX = 10 * ((Input.MouseX / Window.Width) - 0.5f);
            listenerZ = 10 * ((Input.MouseY / Window.Height) - 0.5f);

            _listener.SetPosition(listenerX, 0, listenerZ);
            AudioCTX.SetCurrentListener(_listener);
        }

        public override void Render(double deltaTime)
        {
            CTX.SetDrawColor(0, 0,0,1);
            CTX.DrawCircle(Window.Width / 2, Window.Height / 2, 20);


            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.DrawCircle(Input.MouseX, Input.MouseY, 20);
            CTX.DrawTextAligned("You are here ("+listenerX +"," +listenerZ +")", Input.MouseX, Input.MouseY, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
    }
}
