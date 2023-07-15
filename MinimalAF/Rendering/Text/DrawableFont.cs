﻿using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;


namespace MinimalAF {

    // Fun fact: If Raylib actually had a good text API, I would have stopped working on this framework,
    // because it is most likely better than this one in every other way
    public struct DrawTextOptions {
        /// <summary> The font size </summary>
        public float FontSize = 24;

        /// <summary> Where in the string to start from? (in chars and not unicde code-points)</summary>
        public int Start = 0;

        /// <summary> 
        /// How many unicode code points to draw? 
        /// (Note: Some unicode code-points can span two C# UTF-16 chars in a string. Look up UTF-16 surrogate pairs in your own time)
        /// </summary>
        public int Count = -1;

        /// <summary> Where should it be drawn ? </summary>
        public float X = 0, Y = 0;

        /// <summary>Spacing between lines</summary>
        public float LineSpacing = 0;

        /// <summary>Spacing between letters on the same line</summary>
        public float LetterSpacing = 0;

        /// <summary> 
        /// Text will be wrapped to this width. They are -1 by default to disable word wrap. 
        /// </summary>
        public float Width = -1;

        /// <summary>
        /// How many spaces should a tab be? Any number you pick here will be fiercly debated, so don't overthink it
        /// </summary>
        public int TabSize = 4;

        /// <summary>
        /// Should tabs be 'gridsnapped' to floor(position / TabSize) * TabSize like in the console?
        /// </summary>
        public bool UseTabStops = true;

        /// <summary> 
        /// How should the text be aligned within the Bounds you provided? 
        /// 0 -> Left/Bottom
        /// 0.5f -> Center
        /// 1.0f -> Right/Top
        /// 
        /// (in-between values also supported, of course)
        /// 
        /// TODO: rename to XAlign and YAlign
        /// </summary>
        public float HAlign = 0, VAlign = 0;

        public DrawTextOptions() {}
    }


    public struct DrawableFontOptions {
        /// <summary>The total number of glyphs we hold in memory at once is CacheGridSize squared</summary>
        public int CacheGridSize = 16;

        /// <summary>The size of each 'cell' in the glyph cache is w*w, 
        /// where w = BaseFontSize * m and 1.5f &lt;= m &lt;= 2.0f (I haven't decided what m should be yet :v)
        /// </summary>
        public int BaseFontSize = 24;

        public DrawableFontOptions() {}
    }

    // TODO: better name once we've removed all the Manager BS
    // some common breakpoints or something
    public class DrawableFont : IDisposable {
        FontAtlas _fontAtlas;
        string _fontName;

        public Texture Texture => _fontAtlas.Texture;

        /// <summary>
        /// Crank up the baseAtlasFontSize if you need to draw bigger fonts.
        /// TODO?: image pyramid
        /// TODO: Signed distance field text shader
        /// </summary>
        public DrawableFont(string fontName, DrawableFontOptions options) {
            _fontName = fontName;

            if (options.BaseFontSize <= 0) {
                throw new Exception("Font size too damn small !!!");
            }

            // who knows what this will load? xD
            if (string.IsNullOrEmpty(fontName)) {
                fontName = "";
            }

            _fontAtlas = FontAtlas.CreateFontAtlas(
                new FontImportSettings {
                    FontName = fontName,
                    FontHeight = options.BaseFontSize,
                },
                options.CacheGridSize
            );
        }

        /// <summary>
        /// Gets the next code point in a C# UTF-16 string.
        /// 
        /// I yoinked it from this SO thread here: https://stackoverflow.com/questions/43564445/how-to-map-unicode-codepoints-from-an-utf-16-file-using-c
        /// </summary>
        public (int, int) GetNextCodepoint(string text, int pos) {
            char c1 = text[pos];
            if (c1 >= 0xd800 && c1 < 0xdc00) {
                char c2 = text[pos + 1];
                int codePoint = ((c1 & 0x3ff) << 10) + (c2 & 0x3ff) + 0x10000;
                return (codePoint, 2);
            }

            return (c1, 1);
        }

        /// <summary>
        /// Measure the width of or draw a single line of text.
        /// 
        /// Alignment options is ignorder, because we can't align text we don't know the width of!
        /// </summary>
        /// <returns>(width of the line, number of chars to the start of the next line, num of codepoints that were just iterated)</returns>
        private (float, int, int) MeasureOrDrawLine<Out>(
            ref Out output, string text, DrawTextOptions options, bool isDrawing
        )
            where Out : IGeometryOutput<Vertex> 
        {
            int i = options.Start, codePoints = 0;
            int wantedCodePoints = options.Count < 0 ? text.Length : options.Count;

            float w = 0;
            while(i < text.Length && codePoints < wantedCodePoints) {
                var (codePoint, cpLen) = GetNextCodepoint(text, i);
                i += cpLen;
                codePoints += 1;

                if (
                    codePoint == '\r'
                    // I probably don't want to be backspacing stuff here, even though that is what this char calls for
                    || codePoint == '\b'
                    // TODO: all the other chars I missed
                ) {
                    continue;
                }

                float nextWidth = w;

                if (codePoint == '\n') {
                    break;
                }

                if (codePoint == '\t') {
                    var (spaceWidth, _) = MeasureGlyph((int)' ', options.FontSize);

                    if (options.UseTabStops) {
                        nextWidth += options.TabSize * spaceWidth;
                    } else {
                        int toNextTabStop = options.TabSize - (codePoint % options.TabSize);
                        nextWidth += toNextTabStop * spaceWidth;
                    }
                } else if (isDrawing) {
                    var (width, _) = DrawGlyph(
                        output,
                        codePoint,
                        options.X + w, options.Y,
                        options.FontSize
                    );
                    nextWidth += width;
                } else {
                    var (width, _) = MeasureGlyph(codePoint, options.FontSize);
                    nextWidth += width;
                }

                if (options.Width > 0 && nextWidth > options.Width) {
                    // don't include this codepoint, as it will push us over the width limit
                    codePoints -= 1;
                    i -= cpLen;
                    break;
                }

                w = nextWidth;
            }

            return (w, i, codePoints);
        }

        /// <summary>
        /// This function will draw the text, as per the options.
        /// 
        /// If you have to draw the text backwards or vertically or along a path,
        /// you will just have to do it manually with <see cref="DrawGlyph"/>.
        /// 
        /// Maybe we will have a DrawTextEx in the future? 
        /// But this one here should be enough for things like text editors
        /// </summary>
        public void DrawText<Out>(Out output, string text, DrawTextOptions options)
            where Out : IGeometryOutput<Vertex> 
        {
            MeasureOrDrawText(ref output, text, true, ref options);
        }

        public Vector2 MeasureText<Out>(Out output, string text, DrawTextOptions options)
            where Out : IGeometryOutput<Vertex> {
            return MeasureOrDrawText(ref output, text, false, ref options);
        }

        private Vector2 MeasureOrDrawText<Out>(
            ref Out output, string text, bool isDrawing, ref DrawTextOptions options
        ) 
            where Out : IGeometryOutput<Vertex> 
        {
            // NOTE: it looks like there are still bugs here

            var prevTexture = CTX.Texture.Get();
            if (isDrawing) {
                CTX.Texture.Use(_fontAtlas.Texture);
            }

            var textSize = new Vector2();
            if (isDrawing) {
                var optCopy = options;
                textSize = MeasureOrDrawText(ref output, text, false, ref optCopy);
            }

            int wantedCodePoints = options.Count < 0 ? text.Length : options.Count;
            int initStart = options.Start;
            float lineHeight = options.FontSize;

            // only useful when isDrawing is false
            float realWidth = 0;

            options.Start = initStart;
            options.Y = options.Y + 
                (1.0f - options.VAlign) * (textSize.Y - (lineHeight + options.LineSpacing))
                - (options.VAlign * lineHeight);

            int codePoints = 0;
            float initX = options.X, initY = options.Y;
            while (options.Start < text.Length && codePoints < wantedCodePoints) {
                options.X = initX;

                // TODO: if hAlign is zero we don't need to measure the line
                var (lineWidth, nextLineStart, codePointsInLine) = MeasureOrDrawLine(ref output, text, options, isDrawing:false);
                if (nextLineStart == options.Start) {
                    // Width is too narrow, as this is the only case when MeasureOrDrawLine won't advance nextLineStart
                    break;
                }

                if (isDrawing) {
                    options.X = initX - options.HAlign * lineWidth;
                    MeasureOrDrawLine(ref output, text, options, isDrawing: true);
                }

                realWidth = MathF.Max(realWidth, lineWidth);
                options.Start = nextLineStart;
                codePoints += codePointsInLine;
                options.Y -= lineHeight - options.LineSpacing;
            }

            if (isDrawing) {
                CTX.Texture.Use(prevTexture);
            }

            float realHeight = MathF.Abs(options.Y - initY);

            if (options.Width > 0) {
                return new Vector2(options.Width, realHeight);
            }

            return new Vector2(realWidth, realHeight);
        }

        public Vector2 MeasureGlyph(int codePoint, float fontSize) {
            var normalized = _fontAtlas.GetGlyphNormalizedSize(codePoint);
            return new Vector2(
                normalized.X * fontSize,
                normalized.Y * fontSize
            );
        }

        public Vector2 DrawGlyph<Out>(Out output, int codePoint, float x, float y, float fontSize)
            where Out : IGeometryOutput<Vertex> 
        {
            var (glyph, wasCacheMiss) = _fontAtlas.GetOrAddGlyph(codePoint);
            var glyphWidth = glyph.NormalizedSize.X * fontSize;
            var glyphHeight = glyph.NormalizedSize.Y * fontSize;
            var glyphVertOffset = glyph.NormalizedVerticalOffset * fontSize;

            if (wasCacheMiss) {
                CTX.Flush();
            }

            var rect = new Rect { 
                X0 = x, X1 = x + glyphWidth, 
                Y0 = y - glyphVertOffset, 
                Y1 = y + glyphHeight - glyphVertOffset
            }.Rectified();
            IM.DrawRect(output, rect, glyph.UV);

            return new Vector2(glyphWidth, glyphHeight);
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (disposed)
                return;

            _fontAtlas.Dispose();
            Console.WriteLine("Font destructed");

            disposed = true;
        }

        ~DrawableFont() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
