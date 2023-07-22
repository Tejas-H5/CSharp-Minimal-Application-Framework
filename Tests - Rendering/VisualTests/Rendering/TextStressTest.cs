using MinimalAF;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests {
    public class TextStressTest : IRenderable {
        List<string> _strings = new List<string>();
        float _fontSize = 12;
        DrawableFont _font;
        Random _rand = new Random();


        public TextStressTest() {
            _font = new DrawableFont("Source Code Pro", new DrawableFontOptions { 
            });
        }

        float lastWidth, lastHeight;
        public void Render(AFContext ctx) {
            if (ctx.VW > lastWidth || ctx.VH > lastHeight) {
                lastWidth = ctx.VW; lastHeight = ctx.VH;
                RebuildStrings(ref ctx);
            }

            ctx.SetDrawColor(Color.Black);
            IM.DrawRect(ctx, ctx.Rect);

            ctx.SetDrawColor(Color.Green);
            for (int i = 0; i < _strings.Count; i++) {
                float y = (ctx.VH - i * _fontSize);
                _font.DrawText(ctx, _strings[i], _fontSize, new DrawTextOptions {
                    VAlign = 1, Y = y,
                });
            }
        }

        char GetRandomChar() {
            char nextUnicode = ' ';
            while (!char.IsLetterOrDigit(nextUnicode)) {
                nextUnicode = (char)_rand.Next(0, char.MaxValue);
            }
            return nextUnicode;
        }

        void RebuildStrings(ref AFContext ctx) {
            _strings = new List<string>();
            float y = ctx.VH;
            int safety = 0;
            while(y > 0 && safety++ < 1000000) {
                var sb = new StringBuilder();
                float x = 0;
                while (x < ctx.VW && safety++ < 1000000) {
                    char randomChar = GetRandomChar();
                    sb.Append(randomChar);

                    var size = _font.MeasureGlyph((int)randomChar, _fontSize);
                    x += size.X;
                }
                _strings.Add(sb.ToString());
                y -= _fontSize;
            }
        }
    }
}
