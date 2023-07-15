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
        public Rect UV;
        public Vector2 NormalizedSize;
        public float NormalizedVerticalOffset;
        public int Slot;
        public int CodePoint;
    }

    // concept taken from https://gamedev.stackexchange.com/questions/123978/c-opentk-text-rendering
    public class FontAtlas : IDisposable {
        private static readonly Random rand = new Random();

        private SKTypeface _skTypeface;
        private SKPaint _skPaint;
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

        /// <summary>
        /// returns info about the new glyph, and if the underlying texture has changed or not. 
        /// (This happens in the case of a cache miss, and is useful if we are rendering a series of glyphs
        /// in an immediate mode style with a buffered mesh).
        /// 
        /// If dryRun = true, then nothing actually happens to the glyph cache, and only the info is gotten. 
        /// we will still return whether it would have been a cache miss or not.
        /// </summary>
        public (GlyphInfo, bool) GetOrAddGlyph(int codePoint) {
            if (HasCharacter(codePoint)) {
                // Cache hit
                return (_glyphValues[_codePointGlyphValuesIndices[codePoint]], false);
            }

            // Cache miss

            var slot = GetFreeSlot();
            var glyphInfo = RenderGlyphIntoSlot(codePoint, slot);

            _glyphValues[slot] = glyphInfo;
            _codePointGlyphValuesIndices[codePoint] = slot;

            return (glyphInfo, true);
        }

        public Vector2 GetGlyphNormalizedSize(int codePoint) {
            if (HasCharacter(codePoint)) {
                return _glyphValues[_codePointGlyphValuesIndices[codePoint]].NormalizedSize;
            }

            var codePointStr = MutableUT8String.CodePointToString(codePoint);
            var (w, h, _) = GetGlyphSize(codePointStr);
            var padding = 2 * _fontImportSettings.Padding;
            return new Vector2(
                Normalized(w), Normalized(h)
            );
        }

        (int, int, SKRect) GetGlyphSize(byte[] codePointStr) {
            if (codePointStr.Length > 4) {
                // TODO?: proper unicode validation
                throw new Exception("codePointStr is supposed to be a single unicode utf-8 sequence");
            }

            // TODO: cleanup
            // TODO: reduce the number allocations somehow. IDK how though.

            var w = _skPaint.GetGlyphWidths(codePointStr, out SKRect[] bounds)[0];
            var glyphWidth = (int)MathF.Ceiling(bounds[0].Left + w); 
            // This is wrong, but it is kinda sorta working so it will stay for now.
            // var glyphHeight = _fontImportSettings.FontHeight;
            var glyphHeight = (int)MathF.Ceiling(bounds[0].Height);

            return (glyphWidth, glyphHeight, bounds[0]);
        }

        /// <summary>
        /// This uploads a codepoint slot directly into OpenGL
        /// </summary>
        private GlyphInfo RenderGlyphIntoSlot(int codePoint, int slot) {
            var codePointStr = MutableUT8String.CodePointToString(codePoint);
            var (glyphWidth, glyphHeight, bounds) = GetGlyphSize(codePointStr);
            var padding = _fontImportSettings.Padding;

            var (row, col) = GetSlotRowCol(slot);

            int rowPx = row * _slotSize;
            int colPx = col * _slotSize;

            // I messed up somewhere else, so I have to reverse the rows and cols here
            int glyphX0 = rowPx + _slotSize / 2 - glyphWidth / 2;
            int glyphY0 = colPx + _slotSize / 2 - glyphHeight / 2;
            int glyphX1 = glyphX0 + glyphWidth;
            int glyphY1 = glyphY0 + glyphHeight;

            float glyphOffset = bounds.Bottom;

            var uv = new Rect {
                X0 = glyphX0 / (float)_bitmapSize,
                X1 = glyphX1 / (float)_bitmapSize,
                Y0 = glyphY0 / (float)_bitmapSize,
                Y1 = glyphY1 / (float)_bitmapSize,
            };

            using (var canvas = new SKCanvas(_intermediateGlyphBuffer)) {
                canvas.Clear();

                var lX0 = glyphX0 - rowPx;
                var lX1 = glyphX1 - rowPx;
                var lY0 = _slotSize - (glyphY0 - colPx);
                var lY1 = _slotSize - (glyphY1 - colPx);

                if (!_skPaint.ContainsGlyphs(codePointStr)) {
                    // ask the font manager for a font with that character
                    var fontManager = SKFontManager.Default;
                    if (!_skTypeface.ContainsGlyph(codePoint)) {
                        _skPaint.Typeface = fontManager.MatchCharacter(codePoint);
                    } else {
                        _skPaint.Typeface = _skTypeface;
                    }
                }

                // TODO: fix obsolete thinggo here
                canvas.DrawText(
                    codePointStr,
                    new SKPoint(lX0, lY0 - glyphOffset),
                    _skPaint
                );

                // To debug the bounds
                canvas.DrawLine(
                    new SKPoint { X = lX0, Y = lY0 },
                    new SKPoint { X = lX0, Y = lY1 },
                    _skPaint
                );

                canvas.DrawLine(
                    new SKPoint { X = lX0, Y = lY0 },
                    new SKPoint { X = lX1, Y = lY0 },
                    _skPaint
                );

                canvas.DrawLine(
                    new SKPoint { X = lX1, Y = lY1 },
                    new SKPoint { X = lX0, Y = lY1 },
                    _skPaint
                );

                canvas.DrawLine(
                    new SKPoint { X = lX1, Y = lY1 },
                    new SKPoint { X = lX1, Y = lY0 },
                    _skPaint
                );

                // To debug the font areas
                canvas.DrawLine(
                    new SKPoint { X = _slotSize, Y = 0 },
                    new SKPoint { X = _slotSize, Y = _slotSize },
                    _skPaint
                );

                canvas.DrawLine(
                    new SKPoint { Y = _slotSize, X = 0 },
                    new SKPoint { Y = _slotSize, X = _slotSize },
                    _skPaint
                );
            }

            _texture.UpdateSubImage(rowPx, colPx, _intermediateGlyphBuffer);


            var info = new GlyphInfo {
                CodePoint = codePoint,
                Slot = slot,
                UV = uv,
                NormalizedSize = new Vector2(
                    Normalized(glyphWidth),
                    Normalized(glyphHeight)
                ),
                NormalizedVerticalOffset = Normalized(glyphOffset)
            };

            return info;
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

//            // draw the chars to the bitmap via SKCanvas
//            {
//    float bitmapWidth = (float)bitmap.Width;
//    float bitmapHeight = (float)bitmap.Height;
//    float currentWidth = padding;
//    float vOffset = bitmap.Height - _fontBottom;

//    using var canvas = new SKCanvas(bitmap);



//    // render a line of text to the image
//    // float vOffset = _fontBottom;
//    IterateCodePointStrs(fontManager, utf8Chars, (codePoint, size) => {
//        var codePointStr = MutableUT8String.CodePointToString(codePoint, size);
//        var position = _skStyle.GetGlyphPositions(codePointStr)[0];
//        var width = _skStyle.GetGlyphWidths(codePointStr)[0];
//        currentWidth += width + padding;

//        canvas.DrawText(codePointStr, new SKPoint(currentWidth, vOffset), _skStyle);

//        // assign each character a UV rectangle mapping it to a spot on the canvas
//        {
//            float u0 = currentWidth / bitmapWidth;
//            float v0 = 0;

//            float u1 = (currentWidth + width) / bitmapWidth;
//            float v1 = 1;

//            Rect uv = new Rect(u0, v0, u1, v1).Rectified();
//            _characterQuadCoords[codePoint] = new GlyphInfo {
//                UV = uv
//            };
//        }
//    });
//}