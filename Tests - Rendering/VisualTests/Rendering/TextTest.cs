using MinimalAF;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests
{
	class TextTest : IRenderable {
        char[] _ringBuffer;
        int _ringBufferPos;
        string _builtString;
        Random _rand = new Random();
        DrawableFont _font;

        public TextTest(string fontName = "Consolas") {
            int cacheGridSize = 12;
            _font = new DrawableFont(fontName, new DrawableFontOptions { CacheGridSize = cacheGridSize });
            _ringBuffer = new char[cacheGridSize * cacheGridSize + 4];
            _ringBufferPos = 0;
            _builtString = "";
            RebuildString();
        }

        void RebuildString() {
            var sb = new StringBuilder();
            sb.Append("Start typing : ");
            for (var i = 0; i < _ringBuffer.Length; i++) {
                if (_ringBuffer[i] == 0) {
                    continue;
                }

                sb.Append(_ringBuffer[i]);
            }
            _builtString = sb.ToString();
        }

        void RenderBuiltString(ref AFContext ctx) {
            ctx.SetDrawColor(Color.Green);

            var result = _font.DrawText(ctx, _builtString, 24, new DrawTextOptions {
                X = ctx.VW / 2.0f, Y = ctx.VH,
                VAlign = 1, HAlign = 0.5f,
                Width = ctx.VW - 50,
            });

            if (ctx.CharsJustInputted.Count > 0) {
                for (var i = 0; i < ctx.CharsJustInputted.Count; i++) {
                    char nextUnicode = ' ';
                    while (!char.IsLetterOrDigit(nextUnicode)) {
                        nextUnicode = (char)_rand.Next(0, char.MaxValue);
                    }
                    _ringBuffer[_ringBufferPos] = nextUnicode;
                    _ringBufferPos = (_ringBufferPos + 1) % _ringBuffer.Length;
                }

                RebuildString();
            }
        }


        public void Render(AFContext ctx) {
            ctx.SetDrawColor(Color.Black);
            IM.DrawRect(ctx, 0, 0, ctx.VW, ctx.VH);

            RenderBuiltString(ref ctx);

            ctx.SetTexture(_font.Texture);
            float size = _font.Texture.Height;
            var rect = new Rect {
                X0 = 0.5f * ctx.VW - size/2.0f, X1 = 0.5f * ctx.VW + size/2.0f,
                Y0 = 0.5f * ctx.VH - size/2.0f, Y1 = 0.5f * ctx.VH + size/2.0f
            };
            ctx.SetDrawColor(Color.White);
            IM.DrawRect(ctx, rect);
            ctx.SetTexture(null); ctx.SetDrawColor(Color.Red);
            IM.DrawRectOutline(ctx, 2, rect);

            _font.DrawText(ctx, "cache misses: " + _font.CacheMissCount, 24, new DrawTextOptions {
                X = ctx.VW * 0.5f, Y = 0.0f,
                HAlign = 0.5f, VAlign = 0,
            });
            _font.CacheMissCount = 0;
        }
    }
}
