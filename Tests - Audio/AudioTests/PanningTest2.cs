using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;
using OpenTK.Mathematics;

namespace AudioEngineTests.AudioTests {
    // Test the audio source positioning abilities
    public class PanningTest2 : IRenderable {
        AudioSourceOneShot clackSound;
        AudioListener listener;

        double timer = 0;
        float listenerX, listenerZ;
        float prevListenerX, prevListenerZ;


        public PanningTest2() {
            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(false, false, clip);

            listener = new AudioListener().MakeCurrent();
        }

        public void Render(FrameworkContext ctx) {
            // update
            {
                timer += Time.DeltaTime;
                if (timer > 0.5f) {
                    timer = 0;
                    clackSound.Play();
                }

                prevListenerX = listenerX;
                prevListenerZ = listenerZ;

                listenerX = 10 * ((ctx.MouseX / ctx.VW) - 0.5f);
                listenerZ = 10 * ((ctx.MouseY / ctx.VH) - 0.5f);

                clackSound.Position = new Vector3(listenerX, 0, listenerZ);
                clackSound.Velocity = new Vector3((listenerX - prevListenerX) / Time.DeltaTime, 0, (listenerZ - prevListenerZ) / Time.DeltaTime);
            }

            ctx.SetDrawColor(0, 0, 0, 1);
            IM.Circle(ctx, ctx.VW / 2, ctx.VH / 2, 20);

            ctx.SetDrawColor(1, 0, 0, 1);
            IM.Circle(ctx, ctx.MouseX, ctx.MouseY, 20);
            _font.Draw(ctx, "source is here (" + listenerX + "," + listenerZ + ")", ctx.MouseX, ctx.MouseY, HAlign.Center, VAlign.Bottom);
        }
    }
}
