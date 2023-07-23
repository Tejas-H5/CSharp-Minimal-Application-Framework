using MinimalAF.Rendering;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinimalAF {
    // taken from https://gamedev.stackexchange.com/questions/123978/c-opentk-text-rendering
    public class FontImportSettings {
        // This is the size of the 'base font'. Glyphs will be rendered at this size, and then 
        // scaled up or down based on the font size we want when rendering.
        // TODO: Image pyramid
        // TODO: Signed distance fields
        public int FontHeight = 96;

        public bool BitmapFont = false;
        public string FromFile; //= "joystix monospace.ttf";
        public string FontName = "Consolas";

        //Atlas settings
        public int Padding = 2;
        public bool IsAntiAliased = true;
    }

    public struct GlyphInfo {
        // initialized by RasterizeGlyph
        public Vector2 NormalizedSize;
        public float NormalizedVerticalOffset;
        public int CodePoint;

        // initialized by the thing that places the glyph into the OpenGL texture
        public Rect UV;
        public int Slot;
    }

    // concept taken from https://gamedev.stackexchange.com/questions/123978/c-opentk-text-rendering
    public class FontAtlas : IDisposable {
        private static readonly Random rand = new Random();

        // TODO: check if this really speeds things up or not
        Dictionary<int, SKTypeface> s_resolvedFonts = new Dictionary<int, SKTypeface>();

        struct GlyphRasterResult {
            public SKBitmap Bitmap;
            public GlyphInfo GlyphInfo = new GlyphInfo { };
            public int GlyphWidth = 0;
            public int GlyphHeight = 0;
            public int GlyphX0 = 0;
            public int GlyphY0 = 0;
            public GlyphRasterResult(SKBitmap bitmap) {
                Bitmap = bitmap;
            }
        }
        // TODO: add a max count onto here to prevent people from just rendering every single unicode character and
        // blowing out their memory
        Dictionary<int, GlyphRasterResult> _rasterCache = new Dictionary<int, GlyphRasterResult>();

        private SKTypeface _skTypeface;
        private SKPaint _skPaint;
        SKPaint GetSKPaintWithTheCorrectFont(int codePoint) {
            // ask the font manager for a font with that character
            if (!_skTypeface.ContainsGlyph(codePoint)) {
                if (s_resolvedFonts.ContainsKey(codePoint)) {
                    _skPaint.Typeface = s_resolvedFonts[codePoint];
                } else {
                    var fontManager = SKFontManager.Default;
                    var resolvedTypeFace = fontManager.MatchCharacter(codePoint);
                    _skPaint.Typeface = resolvedTypeFace;
                    s_resolvedFonts[codePoint] = resolvedTypeFace;
                }
            } else {
                _skPaint.Typeface = _skTypeface;
            }

            return _skPaint;
        }


        private FontImportSettings _fontImportSettings;

        private Dictionary<int, int> _codePointGlyphValuesIndices;
        private GlyphInfo[] _glyphValues;
        private int _bitmapSize;
        private int _slotGridSize;
        private int _slotSize;
        private int _numSlots;
        private int _freeSlot = 0;
        private SKBitmap _intermediateGlyphBuffer;

        Texture _texture;
        public Texture Texture => _texture;
        
        public SKTypeface SystemFont {
            get => _skTypeface;
        }

        /// <summary>
        /// May be different to what was specified.
        /// </summary>
        public string FontName => _fontImportSettings.FontName;
        public int FontSize => _fontImportSettings.FontHeight;


        public static FontAtlas CreateFontAtlas(FontImportSettings importSettings, int cacheGridSize) {
            SKTypeface systemFont = TryLoadFont(importSettings);
            if (systemFont == null)
                return null;

            return new FontAtlas(importSettings, systemFont, cacheGridSize);
        }

        /// <summary>
        /// TODO: rename this to GlyphCache. 
        /// 
        /// This class uses SkiaSharp to render glyphs to a bitmap, and implements some sort of 
        /// LRU cache to evect the least recently used glyph. 
        /// 
        /// The texture is synced with OpenGL whenever we encounter a cache miss. 
        /// Cache misses should also flush the current immediate mode mesh buffer after
        /// the OpenGL texter is modified.
        /// </summary>
        private FontAtlas(FontImportSettings importSettings, SKTypeface font, int cacheGridSize) {
            _fontImportSettings = importSettings;
            _skTypeface = font;
            _codePointGlyphValuesIndices = new Dictionary<int, int>();

            importSettings.FontName = font.FamilyName;
            _skPaint = new SKPaint {
                Typeface = _skTypeface,
                TextSize = importSettings.FontHeight,
                Color = new SKColor(0xFF, 0xFF, 0xFF, 0xFF),
                TextAlign = SKTextAlign.Left,
                IsAntialias = importSettings.IsAntiAliased
            };

            _slotGridSize = cacheGridSize;

            // Idk how much larger than the font size this should be to actually fit all the glyphs, but it is what it is.
            // We need to know this here, because later on, we may start using fallback fonts that have different glyph sizes.
            // Need to make sure that the entire character can always be rendered in all the glyphs
            float magicFontSizeMultiplerNumberLmao = 1.5f;
            _slotSize = (int)MathF.Floor(magicFontSizeMultiplerNumberLmao * _fontImportSettings.FontHeight) + 2 * _fontImportSettings.Padding;
            _bitmapSize = _slotSize * _slotGridSize;
            var bitmap = new SKBitmap(_bitmapSize, _bitmapSize);
            _intermediateGlyphBuffer = new SKBitmap(_slotSize, _slotSize);
            using (var canvas = new SKCanvas(bitmap)) {
                canvas.Clear(new SKColor(0, 0, 0, 0));
            }
            _numSlots = _slotGridSize * _slotGridSize;
            _glyphValues = new GlyphInfo[_numSlots];
            _texture = new Texture(
                bitmap,
                new TextureImportSettings {
                    Filtering = FilteringType.Bilinear
                }
            );

            // TODO: Initialize the cache with some commonly used chars
            // var defaultChars = "!#$%&\"()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~'";

#if DEBUG
            //// TODO: REMOVE THIS LATER
            //// Save the texture atlas image as a file so we can see if this works or not
            //using (MemoryStream memStream = new MemoryStream())
            //using (SKManagedWStream wstream = new SKManagedWStream(memStream)) {
            //    var res = _texture.bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
            //    byte[] data = memStream.ToArray();

            //    File.WriteAllBytes($"./debug-{_skStyle.TextSize}-{_skTypeface.FamilyName}.png", data);
            //}
#endif
        }

        public void SaveAtlasAsImage() {
        
        }

        (int, int) GetSlotRowCol(int slot) {
            var row = slot / _slotGridSize;
            var column = slot % _slotGridSize;
            return (row, column);
        }

        GlyphRasterResult RasterizeGlyph(int codePoint, SKBitmap bitmap) {
            var codePointStr = CharArrayList.CodePointToString(codePoint);
            var (glyphWidth, glyphHeight, bounds) = GetGlyphSizeInternal(codePointStr, codePoint);
            var padding = _fontImportSettings.Padding;
            float glyphOffset = bounds.Bottom;

            var lX0 = _slotSize / 2 - glyphWidth / 2;
            var lX1 = lX0 + glyphWidth;

            var lY0NotFlipped = _slotSize / 2 - glyphHeight / 2;
            var lY1NotFlipped = lY0NotFlipped + glyphHeight;
            var lY0 = _slotSize - lY0NotFlipped;
            var lY1 = _slotSize - lY1NotFlipped;

            using (var canvas = new SKCanvas(bitmap)) {
                canvas.Clear();
                var skPaint = GetSKPaintWithTheCorrectFont(codePoint);
                // NOTE: _skPaint.Typeface can potentially be null here

                // TODO: fix obsolete thinggo here
                canvas.DrawText(
                    codePointStr,
                    new SKPoint(lX0, lY0 - glyphOffset),
                    skPaint
                );

#if false

                // To debug the bounds
                canvas.DrawLine(
                    new SKPoint { X = lX0, Y = lY0 },
                    new SKPoint { X = lX0, Y = lY1 },
                    skPaint
                );

                canvas.DrawLine(
                    new SKPoint { X = lX0, Y = lY0 },
                    new SKPoint { X = lX1, Y = lY0 },
                    skPaint
                );

                canvas.DrawLine(
                    new SKPoint { X = lX1, Y = lY1 },
                    new SKPoint { X = lX0, Y = lY1 },
                    skPaint
                );

                canvas.DrawLine(
                    new SKPoint { X = lX1, Y = lY1 },
                    new SKPoint { X = lX1, Y = lY0 },
                    skPaint
                );

                // To debug the font areas
                canvas.DrawLine(
                    new SKPoint { X = _slotSize, Y = 0 },
                    new SKPoint { X = _slotSize, Y = _slotSize },
                    skPaint
                );

                canvas.DrawLine(
                    new SKPoint { Y = _slotSize, X = 0 },
                    new SKPoint { Y = _slotSize, X = _slotSize },
                    skPaint
                );
#endif
            }

            var info = new GlyphInfo {
                CodePoint = codePoint,
                NormalizedSize = new Vector2(
                    Normalized(glyphWidth),
                    Normalized(glyphHeight)
                ),
                NormalizedVerticalOffset = Normalized(glyphOffset)
            };

            return new GlyphRasterResult (bitmap) {
                GlyphInfo = info,
                GlyphWidth = glyphWidth, GlyphHeight = glyphHeight,
                GlyphX0 = lX0, GlyphY0 = lY0NotFlipped
            };
        }


        /// <summary>
        /// This function evicts something from the glyph cache if we don't have any free slots
        /// </summary>
        int GetFreeSlot() {
            if (_freeSlot == _numSlots) {
                // We need to evict the least recently used glyph.
                // But I don't want to wrtie an LRU cache at the moment, so I am just going to evict a random glyph.
                // The LRU cache must be fast in getting an item, and there may be leeway in cache misses.

                var index = rand.Next(0, _numSlots);
                _codePointGlyphValuesIndices.Remove(_glyphValues[index].CodePoint);
                return index;
            }

            return _freeSlot++;
        }

        public (int, bool) GetFreeSlotForGlyph(int codePoint) {
            if (HasCharacter(codePoint)) {
                // Cache hit
                return (_codePointGlyphValuesIndices[codePoint], false);
            }

            // Cache miss

            var slot = GetFreeSlot();

            return (slot, true);
        }

        public GlyphInfo GetGlyphInfo(int slot) {
            return _glyphValues[slot];
        }


        public Vector2 GetGlyphNormalizedSize(int codePoint) {
            if (HasCharacter(codePoint)) {
                return _glyphValues[_codePointGlyphValuesIndices[codePoint]].NormalizedSize;
            }

            var codePointStr = CharArrayList.CodePointToString(codePoint);
            var (w, h, _) = GetGlyphSizeInternal(codePointStr, codePoint);
            return new Vector2(
                Normalized(w), Normalized(h)
            );
        }

        (int, int, SKRect) GetGlyphSizeInternal(byte[] codePointStr, int codePoint) {
            if (codePointStr.Length > 4) {
                // TODO?: proper unicode validation
                throw new Exception("codePointStr is supposed to be a single unicode utf-8 sequence");
            }

            // TODO: cleanup
            // TODO: reduce the number allocations somehow. IDK how though.
            var skPaint = GetSKPaintWithTheCorrectFont(codePoint);
            var w = skPaint.GetGlyphWidths(codePointStr, out SKRect[] bounds)[0];
            var glyphWidth = (int)MathF.Ceiling(bounds[0].Left + w); 
            // This is wrong, but it is kinda sorta working so it will stay for now.
            // var glyphHeight = _fontImportSettings.FontHeight;
            var glyphHeight = (int)MathF.Ceiling(bounds[0].Height);

            return (glyphWidth, glyphHeight, bounds[0]);
        }

        /// <summary>
        /// This uploads a codepoint slot directly into OpenGL
        /// </summary>
        public void RenderGlyphIntoSlot(int codePoint, int slot) {
            GlyphRasterResult res;
            // if (_rasterCache.ContainsKey(codePoint)) {
                //  cache l2 hit
                //res = _rasterCache[codePoint];
            // } else {
                // cache l2 miss
                res = RasterizeGlyph(codePoint, _intermediateGlyphBuffer);
               // _rasterCache[codePoint] = res;
            // }

            var (row, col) = GetSlotRowCol(slot);
            int rowPx = row * _slotSize;
            int colPx = col * _slotSize;

            var glyphInfo = res.GlyphInfo;

            // I messed up somewhere else, so I have to reverse the rows and cols here
            int glyphX0 = rowPx + res.GlyphX0;
            int glyphY0 = colPx + res.GlyphY0;
            int glyphX1 = glyphX0 + res.GlyphWidth;
            int glyphY1 = glyphY0 + res.GlyphHeight;

            glyphInfo.Slot = slot;
            glyphInfo.UV = new Rect {
                X0 = glyphX0 / (float)_bitmapSize,
                X1 = glyphX1 / (float)_bitmapSize,
                Y0 = glyphY0 / (float)_bitmapSize,
                Y1 = glyphY1 / (float)_bitmapSize,
            };

            _texture.UpdateSubImage(rowPx, colPx, _intermediateGlyphBuffer);

            _glyphValues[slot] = glyphInfo;
            _codePointGlyphValuesIndices[codePoint] = slot;
        }

        private float Normalized(float val) {
            return val / (float)_fontImportSettings.FontHeight;
        }

        public bool HasCharacter(int c) {
            return _codePointGlyphValuesIndices.ContainsKey(c);
        }

        private static SKTypeface TryLoadFont(FontImportSettings fontSettings) {
            try {
                SKTypeface skTypeFace;

                if (!string.IsNullOrWhiteSpace(fontSettings.FromFile)) {
                    skTypeFace = SKTypeface.FromFile(fontSettings.FromFile);
                } else {
                    skTypeFace = SKTypeface.FromFamilyName(fontSettings.FontName);
                }

                return skTypeFace;
            } catch {
                return null;
            }
        }

        public void Dispose() {
            _texture.Dispose();
        }
    }
}
