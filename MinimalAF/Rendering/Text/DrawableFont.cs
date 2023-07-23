using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using SkiaSharp.HarfBuzz;
using System.Collections.Generic;


namespace MinimalAF {
    public struct MeasureTextResult {
        /// <summary>
        /// The size of the text's bounding box. We don't return a rect here, because we don't know 
        /// where the text is when we are measuring it, typically
        /// </summary>
        public Rect Bounds = new Rect { };

        /// <summary>
        /// Where was the last character drawn?
        /// </summary>
        public Vector2 LastCursorPos = Vector2.Zero;

        /// <summary>
        /// Where did we reach into the string?
        /// </summary>
        public int StringPosition = 0;

        public MeasureTextResult() {}
    }


    // Fun fact: If Raylib actually had a good text API, I would have stopped working on this framework,
    // because it is most likely better than this one in every other way. It probably even implements
    // the rendering stuff better (I am not a team of google engineers, or even a team), but this API is easier to use
    public struct DrawTextOptions {
        /// <summary> Where in the string to start from? (in chars and not unicde code-points)</summary>
        public int Start = 0;

        /// <summary> 
        /// How many unicode code points to draw? 
        /// (Note: Some unicode code-points can span two C# UTF-16 chars in a string. Look up UTF-16 surrogate pairs in your own time)
        /// </summary>
        public int Count = int.MaxValue;

        /// <summary>
        /// Should the number of lines be limited?
        /// </summary>
        public int LineCount = int.MaxValue;

        /// <summary> Where should it be drawn ? </summary>
        public float X = 0, Y = 0;

        /// <summary>Spacing between lines</summary>
        public float LineSpacing = 0;

        /// <summary>Spacing between letters on the same line</summary>
        public float LetterSpacing = 0;

        /// <summary> 
        /// Text will be wrapped to this width. They are -1 by default to disable word wrap. 
        /// </summary>
        public float Width = float.MaxValue;

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

        public HarfBuzzSharp.Buffer _hbBuffer;

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

            _hbBuffer = new HarfBuzzSharp.Buffer();
            _hbBuffer.Reset();

            // TODO: may need to change based on the language
            _hbBuffer.Direction = HarfBuzzSharp.Direction.LeftToRight;
        }

        /// <summary>
        /// Measure the width of or draw a single line of text.
        /// 
        /// Alignment options is ignorder, because we can't align text we don't know the width of!
        /// </summary>
        /// <returns>(width of the line, number of chars to the start of the next line, num of codepoints that were just iterated)</returns>
        private (float, int, int) MeasureOrDrawLine<Out>(
            ref Out output, ReadOnlySpan<char> text, float fontSize, DrawTextOptions options, bool isDrawing
        )
            where Out : IGeometryOutput<Vertex> 
        {
            int i = options.Start, codePoints = 0;
            int wantedCodePoints = options.Count < 0 ? text.Length : options.Count;

            float lineWidth = 0;
            while(i < text.Length && codePoints < wantedCodePoints) {
                var (codePoint, cpLen) = CharArrayList.GetNextCodepoint(text, i);
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

                if (codePoint == '\n') {
                    break;
                }

                float glyphWidth; /* => */ {
                    if (codePoint == '\t') {
                        var (spaceWidth, _) = MeasureGlyph((int)' ', fontSize);

                        if (options.UseTabStops) {
                            glyphWidth = options.TabSize * spaceWidth;
                        } else {
                            int toNextTabStop = options.TabSize - (codePoint % options.TabSize);
                            glyphWidth = toNextTabStop * spaceWidth;
                        }
                    } else if (isDrawing) {
                        var (width, _) = DrawGlyph(
                            output,
                            codePoint,
                            options.X + lineWidth, options.Y,
                            fontSize
                        );
                        glyphWidth = width;
                    } else {
                        var (width, _) = MeasureGlyph(codePoint, fontSize);
                        glyphWidth = width;
                    }
                }

                if (lineWidth + glyphWidth > options.Width) {
                    codePoints -= 1;
                    i -= cpLen;
                    break;
                }

                lineWidth += glyphWidth;
            }

            return (lineWidth, i, codePoints);
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
        public MeasureTextResult DrawText<Out>(Out output, ReadOnlySpan<char> text, float fontSize, DrawTextOptions options)
            where Out : IGeometryOutput<Vertex> 
        {
            return MeasureOrDrawText(ref output, text, fontSize, true, ref options);
        }

        public MeasureTextResult MeasureText<Out>(Out output, ReadOnlySpan<char> text, float fontSize, DrawTextOptions options)
            where Out : IGeometryOutput<Vertex> {
            return MeasureOrDrawText(ref output, text, fontSize, false, ref options);
        }

        private MeasureTextResult MeasureOrDrawText<Out>(
            ref Out output, ReadOnlySpan<char> text, float fontSize, bool isDrawing, ref DrawTextOptions options
        ) 
            where Out : IGeometryOutput<Vertex> 
        {
            var prevTexture = CTX.Texture.Get();
            if (isDrawing) {
                CTX.Texture.Use(_fontAtlas.Texture);
            }

            var measureResult = new MeasureTextResult { };
            if (isDrawing) {
                var optCopy = options;
                measureResult = MeasureOrDrawText(ref output, text, fontSize, false, ref optCopy);
            }

            int initStart = options.Start;
            float lineHeight = fontSize;

            // only useful when measuring, (when isDrawing is false)
            float realWidth = 0;

            options.Start = initStart;

            // the way the line is vertically anchored changes oppositely to how the text-block is anchored based on VAlign
            options.Y = options.Y + 
                (1.0f - options.VAlign) * (measureResult.Bounds.Height - (lineHeight + options.LineSpacing))
                - (options.VAlign * lineHeight);

            float lineResetX = options.X;

            float initialY = options.Y;
            float minXPosition = options.X;
            int codePointsDrawn = 0, linesDrawn = 0;

            while (
                options.Start < text.Length && 
                codePointsDrawn < options.Count && 
                linesDrawn < options.LineCount
            ) {
                // TODO: if hAlign is zero we don't need to measure the line
                var (lineWidth, nextLineStart, codePointsInLine) = MeasureOrDrawLine(ref output, text, fontSize, options, isDrawing:false);

                if (nextLineStart == options.Start) {
                    // Width is too narrow, as this is the only case when MeasureOrDrawLine won't advance nextLineStart
                    options.Start++;
                    break;
                }

                options.X = lineResetX - options.HAlign * lineWidth;
                minXPosition = MathF.Min(minXPosition, options.X);
                realWidth = MathF.Max(realWidth, lineWidth);

                if (isDrawing) {
                    MeasureOrDrawLine(ref output, text, fontSize, options, isDrawing: true);
                }

                options.Start = nextLineStart;
                options.X = lineResetX;
                options.Y -= lineHeight - options.LineSpacing;
                codePointsDrawn += codePointsInLine;
                linesDrawn += 1;
            }

            if (isDrawing) {
                CTX.Texture.Use(prevTexture);
            }

            measureResult.StringPosition = options.Start;
            measureResult.Bounds = new Rect {
                X0 = minXPosition, X1 = minXPosition + realWidth,
                Y0 = options.Y, Y1 = initialY
            }.Rectified();

            return measureResult;
        }

        public Vector2 MeasureGlyph(int codePoint, float fontSize) {
            var normalized = _fontAtlas.GetGlyphNormalizedSize(codePoint);
            return new Vector2(
                normalized.X * fontSize,
                normalized.Y * fontSize
            );
        }

        public int CacheMissCount = 0;

        public Vector2 DrawGlyph<Out>(Out output, int codePoint, float x, float y, float fontSize)
            where Out : IGeometryOutput<Vertex> 
        {
            var (glyphSlot, wasCacheMiss) = _fontAtlas.GetFreeSlotForGlyph(codePoint);
            if (
                wasCacheMiss
                || CTX.Texture.HasTextureChanged(_fontAtlas.Texture)
            ) {
                CacheMissCount++;
                CTX.Texture.Use(_fontAtlas.Texture);
                CTX.Flush();

                _fontAtlas.RenderGlyphIntoSlot(codePoint, glyphSlot);
            }

            var glyph = _fontAtlas.GetGlyphInfo(glyphSlot);
            var glyphWidth = glyph.NormalizedSize.X * fontSize;
            var glyphHeight = glyph.NormalizedSize.Y * fontSize;
            var glyphVertOffset = glyph.NormalizedVerticalOffset * fontSize;

            y -= glyphVertOffset;
            var rect = new Rect { 
                X0 = x, X1 = x + glyphWidth, 
                Y0 = y, Y1 = y + glyphHeight
            };
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
