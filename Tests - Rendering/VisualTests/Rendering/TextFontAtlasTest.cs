using MinimalAF.Rendering;
using MinimalAF;
using System;

namespace RenderingEngineVisualTests {
    class TextFontAtlasText : IRenderable {
        DrawableFont font;

        public TextFontAtlasText() {
            // font = new DrawableFont("Consolas", 16);
            // font = new DrawableFont("Source Code Pro", 44);
            // font = new DrawableFont("Courier New", 96);
            font = new DrawableFont("", new DrawableFontOptions { BaseFontSize = 33 });

            // "昨夜のコンサートは最高でした уилщертхуилоыхнлойк MR Worldwide 😎😎😎 💯 💯 💯 "
        }

        float pos = 0;
        public void Render(FrameworkContext ctx) {
            pos += 50 * ctx.MouseWheelNotches;

            // var testText = "";
            // var testText = "1234567890=qwertyuiop[[[]asdfghjkl;'\\zxcvbnm,./";
            var testText = "!#$%&\"()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~'昨夜のコンサートは最高でした уилщертхуилоыхнлойк MR Worldwide 😎😎😎 💯 💯 💯 ";

            ctx.SetDrawColor(Color.Black);
            IM.DrawRect(ctx, 0, 0, ctx.VW, ctx.VH);

            ctx.SetDrawColor(Color.White);
            font.DrawText(ctx, testText, new DrawTextOptions {
                FontSize = 24, X = ctx.VW / 2, Y = ctx.VH,
                Width = ctx.VH - 20, // VH not a typo here
                VAlign = 1, HAlign = 0.5f
            });

            var cX = ctx.VW * 0.5f;
            var cY = ctx.VH * 0.5f;
            var rect = new Rect(
                cX - font.Texture.Width / 2f, cY - font.Texture.Height / 2f,
                cX + font.Texture.Width / 2f, cY + font.Texture.Height / 2f
            );

            ctx.SetDrawColor(Color.Red);
            IM.DrawRectOutline(ctx, 2, rect);

            ctx.SetDrawColor(Color.White);
            ctx.SetTexture(font.Texture);
            IM.DrawRect(ctx, rect);
        }
    }
}
