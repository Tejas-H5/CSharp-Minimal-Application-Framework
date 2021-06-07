using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.Rendering.Text
{
    public class FontAtlasTexture
    {
        FontAtlas _fontAtlas;
        Texture _fontTexture;

        public FontAtlasTexture(FontAtlas fontAtlas, Texture fontTexture)
        {
            _fontAtlas = fontAtlas;
            _fontTexture = fontTexture;
        }

        public FontAtlas FontAtlas { get { return _fontAtlas; } }
        public Texture FontTexture { get { return _fontTexture; } }
    }

    public class FontManager : IDisposable
    {
        FontAtlasTexture _activeFont;

        public FontAtlasTexture ActiveFont {
            get {
                return _activeFont;
            }
        }

        Dictionary<string, FontAtlasTexture> _allLoadedFonts = new Dictionary<string, FontAtlasTexture>();

        public Texture UnderlyingFontAtlas {
            get {
                return _activeFont.FontTexture;
            }
        }

        private static string GenerateKey(string fontName, int fontSize)
        {
            return fontName + fontSize.ToString();
        }

        public void SetCurrentFont(string fontName, int fontSize)
        {
            if (string.IsNullOrEmpty(fontName))
            {
                fontName = SystemFonts.DefaultFont.Name;
            }

            if (fontSize <= 0)
            {
                fontSize = 12;
            }

            string key = GenerateKey(fontName, fontSize);
            if (!_allLoadedFonts.ContainsKey(key))
            {
                FontAtlas atlas = FontAtlas.CreateFontAtlas(
                        new FontImportSettings
                        {
                            FontName = fontName,
                            FontSize = fontSize
                        }
                    );

                if (atlas == null)
                {
                    SetCurrentFont("", fontSize);
                    _allLoadedFonts[key] = _activeFont;
                    return;
                }

                Texture texture = new Texture(atlas.Image, new TextureImportSettings { Filtering = FilteringType.NearestNeighbour });
                _allLoadedFonts[key] = new FontAtlasTexture(atlas, texture);
            }

            _activeFont = _allLoadedFonts[key];
        }


        public float GetStringHeight(string s)
        {
            return GetStringHeight(s, 0, s.Length);
        }

        public float GetStringHeight(string s, int start, int end)
        {
            int numNewLines = 1;

            for (int i = start; i < end; i++)
            {
                if (s[i] == '\n')
                    numNewLines++;
            }

            return 2 + numNewLines * (GetCharHeight('|') + 2);
        }


        public float GetStringWidth(string s)
        {
            return GetStringWidth(s, 0, s.Length);
        }

        public float GetStringWidth(string s, int start, int end)
        {
            float maxWidth = 0;
            float width = 0;

            for (int i = start; i < end; i++)
            {
                if (s[i] == '\n')
                {
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
                return _activeFont.FontAtlas.CharWidth;
            }
        }

        public float CharHeight {
            get {
                return _activeFont.FontAtlas.CharHeight;
            }
        }

        public float GetCharWidth(char c)
        {
            if (_activeFont.FontAtlas.IsValidCharacter(c))
            {
                return _activeFont.FontAtlas.GetCharacterSize(c).Width;
            }


            float spaceWidth = _activeFont.FontAtlas.GetCharacterSize('|').Width;

            switch (c)
            {
                case ' ':
                    return spaceWidth;
                case '\t':
                    return 4 * spaceWidth;
            }

            return 0;
        }

        public float GetCharHeight(char c)
        {
            return _activeFont.FontAtlas.GetCharacterSize(c).Height;
        }

        public SizeF GetCharSize(char c)
        {
            return _activeFont.FontAtlas.GetCharacterSize(c);
        }

        public void Dispose()
        {
            foreach (var item in _allLoadedFonts)
            {
                item.Value.FontTexture.Dispose();
            }
        }
    }
}
