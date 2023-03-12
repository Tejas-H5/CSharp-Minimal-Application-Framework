using MinimalAF;
using System;
using System.Text;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Test that the keyboard input is working.",
        tags: "2D, keyboard input"
    )]
    public class KeyboardTest : IRenderable {
        public KeyboardTest(FrameworkContext ctx) {
            var w = ctx.Window;
            w.Size = (800, 600);
            w.Title = "Keyboard test";

            ctx.SetClearColor(Color.White);
        }

        string GetCharactersHeld(ref FrameworkContext ctx) {
            var held = new StringBuilder();
            for (KeyCode key = 0; key < KeyCode.EnumLength; key++) {
                if (ctx.Window.KeyIsDown(key)) {
                    held.Append(CharKeyMapping.KeyCodeToChar(key));
                }
            }

            return held.ToString();
        }

        string GetCharactersReleased(ref FrameworkContext ctx) {
            var held = new StringBuilder();
            for (KeyCode key = 0; key < KeyCode.EnumLength; key++) {
                if (ctx.Window.KeyJustReleased(key)) {
                    held.Append(CharKeyMapping.KeyCodeToChar(key));
                }
            }

            return held.ToString();
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(0, 0, 0, 1);
            var w = ctx.Window;


            var keys = GetCharactersReleased(ref ctx);
            if (w.KeyIsDown(KeyCode.Shift)) {
                keys = keys.ToUpper();
            }

            if (w.KeyIsDown(KeyCode.Any)) {

                Console.WriteLine("PRessed: " + GetCharactersHeld(ref ctx));
            }

            if (w.KeyJustReleased(KeyCode.Any)) {
                Console.WriteLine("Released: " + GetCharactersReleased(ref ctx));
            }

            ctx.DrawText("Press some keys:", ctx.VW * 0.5f, ctx.VH * 0.75f, HAlign.Center, VAlign.Center);
            ctx.DrawText(keys, ctx.VW / 2, ctx.VH / 2, HAlign.Center, VAlign.Center);
        }
    }
}
