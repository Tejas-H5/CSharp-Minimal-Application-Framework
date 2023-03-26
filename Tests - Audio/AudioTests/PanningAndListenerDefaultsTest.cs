using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    // Test the audio listner positioning abilities
    public class PanningAndListenerDefaultsTest : IRenderable {
        AudioSourceOneShot clackSound;
        AudioListener listener;

        public PanningAndListenerDefaultsTest() {
            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(false, false, clip);

            listener = new AudioListener().MakeCurrent();
        }

        double timer = 0;
        float listenerX, listenerZ;

        public void Render(FrameworkContext ctx) {
            // updating something ?? idk I forgot
            {
                timer += Time.DeltaTime;
                if (timer > 0.5f) {
                    timer = 0;
                    clackSound.Play();
                }

                listenerX = 10 * ((ctx.MouseX / ctx.VW) - 0.5f);
                listenerZ = 10 * ((ctx.MouseY / ctx.VH) - 0.5f);

                listener.SetPosition(listenerX, 0, listenerZ);
            }

            ctx.SetDrawColor(0, 0, 0, 1);
            IM.DrawCircle(ctx, ctx.VW / 2, ctx.VH / 2, 20);


            ctx.SetDrawColor(1, 0, 0, 1);
            IM.DrawCircle(ctx, ctx.MouseX, ctx.MouseY, 20);
            _font.Draw(ctx, "You are here (" + listenerX + "," + listenerZ + ")", ctx.MouseX, ctx.MouseY, HAlign.Center, VAlign.Bottom);
        }
    }
}
