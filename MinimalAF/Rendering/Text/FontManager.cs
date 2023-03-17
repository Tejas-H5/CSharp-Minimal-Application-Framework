using OpenTK.Mathematics;
using System;
using System.Collections.Generic;


namespace MinimalAF.Rendering.Text {
    public class FontAtlasTexture {
        FontAtlas fontAtlas;
        Texture fontTexture;
        public string FontName => fontAtlas.FontName;
        public int FontSize => fontAtlas.FontSize;

        public FontAtlasTexture(FontAtlas fontAtlas, Texture fontTexture) {
            this.fontAtlas = fontAtlas;
            this.fontTexture = fontTexture;
        }

        public FontAtlas FontAtlas {
            get {
                return fontAtlas;
            }
        }
        public Texture FontTexture {
            get {
                return fontTexture;
            }
        }
    }

    public class FontManager : IDisposable {
        FontAtlasTexture activeFont;

        public FontAtlasTexture ActiveFont {
            get {
                return activeFont;
            }
        }

        Dictionary<string, FontAtlasTexture> allLoadedFonts = new Dictionary<string, FontAtlasTexture>();

        public Texture UnderlyingFontAtlas {
            get {
                return activeFont.FontTexture;
            }
        }

        private static string GenerateKey(string fontName, int fontSize) {
            return fontName + fontSize.ToString();
        }

        public void SetCurrentFont(string fontName, int fontSize) {
            if (string.IsNullOrEmpty(fontName)) {
                fontName = "";
            }

            if (fontSize <= 0) {
                fontSize = 16;
            }

            string key = GenerateKey(fontName, fontSize);
            if (!allLoadedFonts.ContainsKey(key)) {
                FontAtlas atlas = FontAtlas.CreateFontAtlas(
                        new FontImportSettings {
                            FontName = fontName,
                            FontSize = fontSize
                        }
                    );

                if (atlas == null) {
                    SetCurrentFont("", fontSize);
                    allLoadedFonts[key] = activeFont;
                    return;
                }

                string actualKey = GenerateKey(atlas.FontName, fontSize);
                if (allLoadedFonts.ContainsKey(actualKey)) {
                    activeFont = allLoadedFonts[actualKey];
                    return;
                }

                Texture texture = new Texture(
                    atlas.Image,
                    new TextureImportSettings {
                        Filtering = FilteringType.Bilinear
                    }
                );
                FontAtlasTexture newFontAtlas = new FontAtlasTexture(atlas, texture);

                allLoadedFonts[key] = newFontAtlas;
                allLoadedFonts[actualKey] = newFontAtlas;
            }

            activeFont = allLoadedFonts[key];
        }


        public float GetStringHeight(string s) {
            return GetStringHeight(s, 0, s.Length);
        }

        public float GetStringHeight(string s, int start, int end) {
            int numNewLines = 1;

            for (int i = start; i < end; i++) {
                if (s[i] == '\n')
                    numNewLines++;
            }

            return 2 + numNewLines * (GetCharHeight('|') + 2);
        }


        public float GetStringWidth(string s) {
            return GetStringWidth(s, 0, s.Length);
        }

        public float GetStringWidth(string s, int start, int end) {
            float maxWidth = 0;
            float width = 0;

            for (int i = start; i < end; i++) {
                if (s[i] == '\n') {
                    width = 0;
                    continue;
                }

                width += GetCharWidth(s[i]);

                maxWidth = MathF.Max(width, maxWidth);
            }

            return maxWidth;
        }


        public float CharWidth {
            get {
                return activeFont.FontAtlas.CharWidth;
            }
        }

        public float CharHeight {
            get {
                return activeFont.FontAtlas.CharHeight;
            }
        }

        public float GetCharWidth(char c) {
            if (activeFont.FontAtlas.HasCharacter(c)) {
                return activeFont.FontAtlas.GetCharacterSize(c).X;
            }


            float spaceWidth = activeFont.FontAtlas.GetCharacterSize('|').X;

            switch (c) {
                case ' ':
                    return spaceWidth;
                case '\t':
                    return 4 * spaceWidth;
            }

            return 0;
        }

        public float GetCharHeight(char c) {
            return activeFont.FontAtlas.GetCharacterSize(c).Y;
        }

        public Vector2 GetCharSize(char c) {
            return activeFont.FontAtlas.GetCharacterSize(c);
        }

        public void Dispose() {
            foreach (var item in allLoadedFonts) {
                item.Value.FontTexture.Dispose();
            }
        }
    }
}
