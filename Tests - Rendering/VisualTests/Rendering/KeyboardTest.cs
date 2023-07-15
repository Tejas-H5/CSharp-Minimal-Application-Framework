using MinimalAF;
using System;
using System.Text;

namespace RenderingEngineVisualTests {
    public class KeyboardTest : IRenderable {
        DrawableFont _font = new DrawableFont("Source code pro", new DrawableFontOptions { });

        string GetCharactersHeld(ref FrameworkContext ctx) {
            var held = new StringBuilder();
            for (KeyCode key = 0; key < KeyCode.EnumLength; key++) {
                if (ctx.KeyIsDown(key)) {
                    held.Append(key.ToString());
                    held.Append(" ");
                }
            }

            return held.ToString();
        }

        string GetCharactersReleased(ref FrameworkContext ctx) {
            var held = new StringBuilder();
            for (KeyCode key = 0; key < KeyCode.EnumLength; key++) {
                if (ctx.KeyJustReleased(key)) {
                    held.Append(key.ToString());
                    held.Append(" ");
                }
            }

            return held.ToString();
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetDrawColor(0, 0, 0, 1);

            if (ctx.KeyIsDown(KeyCode.Any)) {
                Console.WriteLine("Pressed: " + GetCharactersHeld(ref ctx));
            }

            if (ctx.KeyJustReleased(KeyCode.Any)) {
                Console.WriteLine("Released: " + GetCharactersReleased(ref ctx));
            }

            var keys = GetCharactersHeld(ref ctx);
            if (ctx.KeyIsDown(KeyCode.Shift)) {
                keys = keys.ToUpper();
            }

            _font.DrawText(ctx, "Press some keys:", new DrawTextOptions {
                X = ctx.VW * 0.5f, Y = ctx.VH * 0.75f,
                HAlign = 0.5f, VAlign = 0.5f
            });

            _font.DrawText(ctx, keys, new DrawTextOptions {
                X = ctx.VW / 2, Y = ctx.VH / 2, HAlign = 0.5f, VAlign = 0.5f
            });
        }
    }
}
