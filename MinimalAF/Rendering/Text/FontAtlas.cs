using MinimalAF.Rendering;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinimalAF {
    //taken from https://gamedev.stackexchange.com/questions/123978/c-opentk-text-rendering
    public class FontImportSettings {
        public static string Text = "GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);";

        //Font import settings
        public int FontSize = 14;
        public bool BitmapFont = false;
        public string FromFile; //= "joystix monospace.ttf";
        public string FontName = "Consolas";

        //Atlas settings
        public int Padding = 2;
        public bool IsAntiAliased = true;
    }

    //concept taken from https://gamedev.stackexchange.com/questions/123978/c-opentk-text-rendering
    public class FontAtlas : IDisposable {
        public const string DefaultCharacters = "!#$%&\"()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~'";
        public const float QUALITY_SCALE_FACTOR = 1f;

        private SKTypeface _skTypeface;
        private SKPaint _skStyle;
        private float _fontBottom;
        private SKBitmap _bitmap;
        private Dictionary<int, Rect> _characterQuadCoords;
        private FontImportSettings _importSettings;

        Texture _texture;
        public Texture Texture {
            get {
                if (_texture == null) {
                    _texture = new Texture(
                        _bitmap,
                        new TextureImportSettings {
                            Filtering = FilteringType.Bilinear
                        }
                    );
                }

                return _texture;
            }
        }


        public SKTypeface SystemFont {
            get => _skTypeface;
        }
        public SKBitmap Image {
            get => _bitmap;
        }
        public float CharWidth => GetCharacterSize('?').X;
        public float CharHeight => GetCharacterSize('?').Y;

        /// <summary>
        /// May be different to what was specified.
        /// </summary>
        public string FontName => _importSettings.FontName;
        public int FontSize => _importSettings.FontSize;


        public static FontAtlas CreateFontAtlas(FontImportSettings importSettings, string characters = DefaultCharacters) {
            SKTypeface systemFont = TryLoadFont(importSettings);
            if (systemFont == null)
                return null;

            return new FontAtlas(importSettings, systemFont, characters);
        }

        private FontAtlas(FontImportSettings importSettings, SKTypeface font, string characters) {
            this._importSettings = importSettings;
            importSettings.FontName = font.FamilyName;
            _skTypeface = font;
            _skStyle = new SKPaint {
                Typeface = _skTypeface,
                TextSize = importSettings.FontSize * QUALITY_SCALE_FACTOR,
                Color = new SKColor(0xFF, 0xFF, 0xFF, 0xFF),
                TextAlign = SKTextAlign.Left,
                IsAntialias = importSettings.IsAntiAliased
            };

            // edge-case rendering code relies on the question mark always being a valid character
            // TODO: use that wierd diamond + question mark character thinggy
            if (!characters.Contains('?', StringComparison.Ordinal))
                characters += '?';

            int padding = importSettings.Padding;

            _characterQuadCoords = new Dictionary<int, Rect>();
            _bitmap = RenderAtlas(characters, _characterQuadCoords, padding);

#if DEBUG
            //// TODO: REMOVE THIS LATER, ITS JUST FOR DEBUG
            //// Create the file.
            //using (MemoryStream memStream = new MemoryStream())
            //using (SKManagedWStream wstream = new SKManagedWStream(memStream)) {
            //    var res = _bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
            //    byte[] data = memStream.ToArray();

            //    File.WriteAllBytes($"./debug-{_skStyle.TextSize}-{_skTypeface.FamilyName}.png", data);
            //}
#endif
        }

        public Rect GetCharacterUV(int codePoint) {
            if (!HasCharacter(codePoint)) {
                codePoint = '?';
            }

            return _characterQuadCoords[codePoint];
        }

        public Vector2 GetCharacterSize(char c) {
            Rect normalized = GetCharacterUV(c);
            float width = _bitmap.Width;
            float height = _bitmap.Height;

            return new Vector2(
                normalized.Width * width / QUALITY_SCALE_FACTOR,
                normalized.Height * height / QUALITY_SCALE_FACTOR
            );
        }

        public bool HasCharacter(int c) {
            return _characterQuadCoords.ContainsKey(c);
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

        // TODO: better name
        private void IterateCodePointStrs(SKFontManager fontManager, MutableUT8String utf8Chars, Action<int, int> fn) {
            int codePoint = 0, size = 0;
            for (int i = 0, pos = 0; pos < utf8Chars.Length; i++, pos += size) {
                (size, codePoint) = utf8Chars.GetNextCodePoint(pos);

                SKTypeface typeFaceToUse;
                if (_skTypeface.ContainsGlyph(codePoint)) {
                    typeFaceToUse = _skTypeface;
                } else {
                    typeFaceToUse = fontManager.MatchCharacter(codePoint);
                }
                _skStyle.Typeface = typeFaceToUse;

                fn(codePoint, size);
            }
        }

        private SKBitmap RenderAtlas(string characters, Dictionary<int, Rect> coordMap, int padding) {
            // Ensure that our font has all of the chosen characters available
            var utf8Chars = new MutableUT8String(characters);
            var fontManager = SKFontManager.Default;

            // calculate the SKBitmap dimensions, and create it
            SKBitmap bitmap;
            float fontHeight = 0;   // TODO: move into scope if it is unused
            {
                float fontBottomMaximum = 0,
                        fontBottomCount = 0,
                        fontBottomSum = 0;
                float bitmapWidth, bitmapHeight;
                float currentWidth = padding;
                IterateCodePointStrs(fontManager, utf8Chars, (codePoint, size) => {
                    var codePointStr = MutableUT8String.CodePointToString(codePoint, size);
                    var position = _skStyle.GetGlyphPositions(codePointStr)[0];
                    var width = _skStyle.GetGlyphWidths(codePointStr)[0];
                    currentWidth += width + padding;

                    _skStyle.GetFontMetrics(out SKFontMetrics metrics);

                    float charHeight = metrics.Bottom - metrics.Top;
                    if (fontHeight < charHeight) {
                        fontHeight = charHeight;
                    }

                    if (metrics.Bottom > fontBottomMaximum) {
                        fontBottomMaximum = metrics.Bottom;
                        fontBottomSum += metrics.Bottom;
                        fontBottomCount++;
                    }
                });

                _fontBottom = fontBottomSum / fontBottomCount;

                fontHeight += 2 * padding;
                

                bitmapWidth = currentWidth;
                bitmapHeight = fontHeight;
                bitmap = new SKBitmap((int)MathF.Ceiling(currentWidth), (int)MathF.Ceiling(fontHeight));
            }

            // draw the chars to the bitmap via SKCanvas
            {
                float bitmapWidth = (float)bitmap.Width;
                float bitmapHeight = (float)bitmap.Height;
                float currentWidth = padding;
                float vOffset = bitmap.Height - _fontBottom;

                using var canvas = new SKCanvas(bitmap);
                
                canvas.Clear(new SKColor(0, 0, 0, 0));

                // render a line of text to the image
                // float vOffset = _fontBottom;
                IterateCodePointStrs(fontManager, utf8Chars, (codePoint, size) => {
                    var codePointStr = MutableUT8String.CodePointToString(codePoint, size);
                    var position = _skStyle.GetGlyphPositions(codePointStr)[0];
                    var width = _skStyle.GetGlyphWidths(codePointStr)[0];
                    currentWidth += width + padding;

                    canvas.DrawText(codePointStr, new SKPoint(currentWidth, vOffset), _skStyle);

                    // assign each character a UV rectangle mapping it to a spot on the canvas
                    {
                        float u0 = currentWidth / bitmapWidth;
                        float v0 = 0;

                        float u1 = (currentWidth + width) / bitmapWidth;
                        float v1 = 1;

                        Rect uv = new Rect(u0, v0, u1, v1).Rectified();
                        coordMap[codePoint] = uv;
                    }
                });
            }

            return bitmap;
        }

        public void Dispose() {
            _texture.Dispose();
        }
    }
}
