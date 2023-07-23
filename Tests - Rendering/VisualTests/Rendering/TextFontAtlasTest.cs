using MinimalAF.Rendering;
using MinimalAF;
using OpenTK.Mathematics;
using System;

namespace RenderingEngineVisualTests {
    class TextFontAtlasText : IRenderable {
        DrawableFont font;

        public TextFontAtlasText() {
            // font = new DrawableFont("Consolas", 16);
            font = new DrawableFont("Source Code Pro", new DrawableFontOptions { BaseFontSize = 96, CacheGridSize=5 });
            // font = new DrawableFont("Courier New", 96);
            // font = new DrawableFont("", new DrawableFontOptions { BaseFontSize = 33 });

            // "昨夜のコンサートは最高でした уилщертхуилоыхнлойк MR Worldwide 😎😎😎 💯 💯 💯 "
        }

        float pos = 0;
        public void Render(AFContext ctx) {
            pos += 50 * ctx.MouseWheelNotches;

            // var testText = "";
            // var testText = "1234567890=qwertyuiop[[[]asdfghjkl;'\\zxcvbnm,./";
            var testText = "!#$%&\"()*+,-./ 😎😎😎 💯 0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~'昨夜のコンサートは最高でした уилщертхуилоыхнлойк MR Worldwide 😎😎😎 💯 💯 💯 ";

            ctx.SetDrawColor(Color.Black);
            IM.DrawRect(ctx, 0, 0, ctx.VW, ctx.VH);

            ctx.SetDrawColor(Color.White);
            font.DrawText(ctx, testText, 24, new DrawTextOptions {
                X = ctx.VW / 2, Y = ctx.VH,
                Width = ctx.VH - 20, // VH not a typo here
                VAlign = 1, HAlign = 0.5f
            });

            var mx = -1.2f * (-1 + 2 * ctx.MouseX / ctx.VW) * font.Texture.Height;
            var my = -1.2f * (-1 + 2 * ctx.MouseY / ctx.VH) * font.Texture.Height;

            var prev = ctx.GetModelMatrix();
            ctx.SetTransform(prev * Matrix4.CreateTranslation(new Vector3(mx, my, 0)));

            var rect = new Rect {
                X0 = 0, X1 = font.Texture.Height,
                Y0 = 0, Y1 = font.Texture.Height
            };

            ctx.SetTexture(null);
            ctx.SetDrawColor(Color.Red);
            IM.DrawRectOutline(ctx, 2, rect);

            ctx.SetDrawColor(Color.White);
            ctx.SetTexture(font.Texture);
            IM.DrawRect(ctx, rect);

            ctx.SetTransform(prev);
        }
    }
}
