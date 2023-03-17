using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinimalAF.Rendering.Text {
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
    public class FontAtlas {
        public const string DefaultCharacters = "!#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~'";
        private SKTypeface _skTypeface;
        private SKPaint _skStyle;
        private float _fontBottom;
        private SKBitmap _bitmap;
        private Dictionary<char, Rect> _characterQuadCoords;
        private FontImportSettings _importSettings;

        const float QUALITY_SCALE_FACTOR = 1f;

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
            if (!characters.Contains('?', StringComparison.Ordinal))
                characters += '?';

            int padding = importSettings.Padding;

            _characterQuadCoords = new Dictionary<char, Rect>();
            _bitmap = RenderAtlas(characters, _characterQuadCoords, padding);

#if DEBUG
            // TODO: REMOVE THIS LATER, ITS JUST FOR DEBUG
            // Create the file.
            using (MemoryStream memStream = new MemoryStream())
            using (SKManagedWStream wstream = new SKManagedWStream(memStream)) {
                var res = _bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] data = memStream.ToArray();

                File.WriteAllBytes($"./debug-{_skStyle.TextSize}-{_skTypeface.FamilyName}.png", data);
            }
#endif
        }

        public Rect GetCharacterUV(char c) {
            if (!HasCharacter(c)) {
                c = '?';
            }

            return _characterQuadCoords[c];
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

        public bool HasCharacter(char c) {
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

        private SKBitmap RenderAtlas(string characters, Dictionary<char, Rect> coordMap, int padding) {
            var positions = _skStyle.GetGlyphPositions(characters);
            var widths = _skStyle.GetGlyphWidths(characters);

            var (bitmapWidth, bitmapHeight) = CalculateImageDimensions(positions, widths, padding);
            var bitmap = new SKBitmap(bitmapWidth, bitmapHeight);

            using (var canvas = new SKCanvas(bitmap)) {
                float bitmapWidthF = bitmapWidth;
                float bitmapHeightF = bitmapHeight;

                canvas.Clear(new SKColor(0,0,0,0));

                // render a line of text to the image
                float vOffset = bitmapHeightF - _fontBottom;

                for(int i = 0; i < characters.Length; i++) {
                    float currentX = positions[i].X + padding * i;
                    canvas.DrawText(characters[i].ToString(), new SKPoint(currentX, vOffset), _skStyle);
                }

                // assign each character a UV rectangle mapping it to a spot on the canvas
                for (int i = 0; i < characters.Length; i++) {
                    float currentX = positions[i].X + padding * i;
                    float width = widths[i];
                    char c = characters[i];

                    float u0 = currentX / bitmapWidthF;
                    float v0 = 0;

                    float u1 = (currentX + width) / bitmapWidthF;
                    float v1 = 1;

                    Rect uv = new Rect(u0, v0, u1, v1).Rectified();
                    coordMap[c] = uv;
                }
            }

            return bitmap;
        }

        private (int, int) CalculateImageDimensions(SKPoint[] positions, float[] widths, int padding) {
            var lastCharWidth = widths[widths.Length - 1];
            float width = positions[positions.Length - 1].X + lastCharWidth 
                + padding + widths.Length * padding;

            _skStyle.GetFontMetrics(out SKFontMetrics metrics);

            float extents = metrics.Bottom - metrics.Top;
            _fontBottom = metrics.Bottom;

            return ((int)MathF.Ceiling(width), (int)MathF.Ceiling(extents));
        }
    }
}
