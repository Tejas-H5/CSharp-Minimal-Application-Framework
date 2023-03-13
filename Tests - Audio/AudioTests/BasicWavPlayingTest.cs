using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    public class BasicWavPlayingTest : IRenderable {
        AudioSourceOneShot clackSound;

        public BasicWavPlayingTest() {
            AudioClipOneShot clip = AudioClipOneShot.FromFile("./Res/keyboardClack0.wav");
            clackSound = new AudioSourceOneShot(true, false, clip);
        }

        string GetCharactersHeld(ref FrameworkContext ctx) {
            var held = new StringBuilder();
            for (KeyCode key = 0; key < KeyCode.EnumLength; key++) {
                if (ctx.KeyIsDown(key)) {
                    held.Append(CharKeyMapping.KeyCodeToChar(key));
                }
            }

            return held.ToString();
        }

        string GetCharactersReleased(ref FrameworkContext ctx) {
            var held = new StringBuilder();
            for (KeyCode key = 0; key < KeyCode.EnumLength; key++) {
                if (ctx.KeyJustReleased(key)) {
                    held.Append(CharKeyMapping.KeyCodeToChar(key));
                }
            }

            return held.ToString();
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(0, 0, 0, 1);

            if (ctx.KeyJustPressed(KeyCode.Any) || ctx.KeyJustReleased(KeyCode.Any)) {
                Console.WriteLine("Pressed: " + GetCharactersHeld(ref ctx));
                clackSound.Play();
            }

            if (ctx.KeyJustReleased(KeyCode.Any)) {
                Console.WriteLine("Released: " + GetCharactersReleased(ref ctx));
            }

            var keys = GetCharactersHeld(ref ctx);
            if (ctx.KeyIsDown(KeyCode.Shift)) {
                keys = keys.ToUpper();
            }

            ctx.DrawText("Press some keys:", ctx.VW * 0.5f, ctx.VH * 0.75f, HAlign.Center, VAlign.Center);
            ctx.DrawText(keys, ctx.VW / 2, ctx.VH / 2, HAlign.Center, VAlign.Center);
        }
    }
}
