using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;


namespace MinimalAF {
    // TODO: better name once we've removed all the Manager BS
    // some common breakpoints or something
    public class DrawableFont : IDisposable {
        FontAtlas _currentFontAtlas;
        string _fontName;

        public Texture Texture => _currentFontAtlas.Texture;

        public float CharWidth => _currentFontAtlas.CharWidth;
        public float CharHeight => _currentFontAtlas.CharHeight;

        // TODO: implement some sort of font-pyramid, so we don't regenerate a font atlas and such for EVERY integer size.
        // Our current approach is really bad. It may even be better to just have 1 really big font size and use that for all
        // font sizes.
        Dictionary<int, FontAtlas> _atlasForSize = new Dictionary<int, FontAtlas>();


        public DrawableFont(string fontName, int fontSize) {
            _fontName = fontName;

            if (fontSize <= 0) {
                // throw new Exception("Font size too damn small !");
                fontSize = 16;
            }

            // who knows what this will load? xD
            if (string.IsNullOrEmpty(fontName)) {
                fontName = "";
            }

            _currentFontAtlas = FontAtlas.CreateFontAtlas(
                new FontImportSettings {
                    FontName = fontName,
                    FontSize = fontSize
                }
            );

        }

        public void SetSize(int size) {
            if (_atlasForSize.ContainsKey(size)) {
                _currentFontAtlas = _atlasForSize[size];
                return;
            }

            _currentFontAtlas = FontAtlas.CreateFontAtlas(
                new FontImportSettings {
                    FontName = _fontName,
                    FontSize = size
                }
            );
            _atlasForSize[size] = _currentFontAtlas;
        }

        public float GetWidth() {
            return GetWidth(' ');
        }

        public float GetHeight() {
            return GetHeight('|');
        }

        public float GetWidth(char c) {
            if (_currentFontAtlas.HasCharacter(c)) {
                return _currentFontAtlas.GetCharacterSize(c).X;
            }

            float spaceWidth = _currentFontAtlas.GetCharacterSize('|').X;

            switch (c) {
                case ' ':
                    return spaceWidth;
                case '\t':
                    return 4 * spaceWidth;  // lmao this isnt how tabs work. TODO: tab stops
            }

            return 0;
        }

        public float GetHeight(char c) {
            return _currentFontAtlas.GetCharacterSize(c).Y;
        }

        public Vector2 GetSize(char c) {
            return new Vector2(GetWidth(c), GetHeight(c));
        }

        private float CaratPosX(float lineWidth, HAlign hAlign) {
            switch (hAlign) {
                case HAlign.Center:
                    return -lineWidth / 2f;
                case HAlign.Right:
                    return -lineWidth;
                default:
                    return 0;
            }
        }

        //TODO: IMPLEMENT tabs and newlines
        //And vertical/horizontal aiignment features
        public Vector2 Draw<Out>(Out output, string text, float startX, float startY, HAlign hAlign, VAlign vAlign, float scale = 1.0f) where Out : IGeometryOutput<Vertex> {
            var prevTexture = CTX.Texture.Get();

            Vector2 caratPos = (startX, startY);

            // don't early return out of here plz
            CTX.Texture.Use(_currentFontAtlas.Texture);
            {

                if (text == null)
                    return caratPos;

                float textHeight = scale * GetStringHeight(text);
                float charHeight = scale * GetHeight('|');

                switch (vAlign) {
                    case VAlign.Bottom:
                        caratPos.Y = startY + textHeight - charHeight;
                        break;
                    case VAlign.Center:
                        caratPos.Y = startY + textHeight / 2f - charHeight;
                        break;
                    case VAlign.Top:
                        caratPos.Y = startY - charHeight;
                        break;
                }

                int lineStart = 0;
                int lineEnd = 0;

                while (lineEnd < text.Length) {
                    lineEnd = text.IndexOf('\n', lineStart);
                    if (lineEnd == -1)
                        lineEnd = text.Length;
                    else
                        lineEnd++;

                    float lineWidth = scale * GetStringWidth(text, lineStart, lineEnd);

                    caratPos.X = startX + CaratPosX(lineWidth, hAlign);

                    caratPos = Draw(output, text, lineStart, lineEnd, caratPos.X, caratPos.Y, scale);

                    lineStart = lineEnd;
                }
            }
            CTX.Texture.Use(prevTexture);


            return caratPos;
        }

        public Vector2 Draw<Out>(Out output, string text, float startX, float startY, float scale = 1.0f) where Out : IGeometryOutput<Vertex> {
            return Draw(output, text, 0, text.Length, startX, startY, scale);
        }

        public Vector2 Draw<Out>(Out output, string text, int start, int end, float startX, float startY, float scale) where Out : IGeometryOutput<Vertex> {
            Texture previousTexture = CTX.Texture.Get();

            CTX.Texture.Use(Texture);

            float x = startX;
            float y = startY;

            for (int i = start; i < end; i++) {
                char c = text[i];

                if (c == '\n') {
                    x = startX;
                    y -= _currentFontAtlas.CharHeight + 2;
                } else {
                    DrawChar(output, c, x, y, scale);
                }

                x += GetWidth(c);
            }

            CTX.Texture.Use(previousTexture);

            return new Vector2(x, y);
        }

        public Vector2 DrawChar<Out>(Out output, char c, float x, float y, float scale = 1) where Out : IGeometryOutput<Vertex> {
            Vector2 size = GetSize(c);

            if (_currentFontAtlas.HasCharacter(c)) {
                Rect uv = _currentFontAtlas.GetCharacterUV(c);
                IM.Rect(output, new Rect(x, y, x + size.X * scale, y + size.Y * scale), uv);
            } else if (c != ' ' && c != '\t') {
                return DrawChar(output, '?', x, y, scale);
            }

            return size;
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

        public float GetCharWidth(char c) {
            if (_currentFontAtlas.HasCharacter(c)) {
                return _currentFontAtlas.GetCharacterSize(c).X;
            }

            float spaceWidth = _currentFontAtlas.GetCharacterSize('|').X;

            switch (c) {
                case ' ':
                    return spaceWidth;
                case '\t':
                    return 4 * spaceWidth;
            }

            return 0;
        }

        public float GetCharHeight(char c) {
            return _currentFontAtlas.GetCharacterSize(c).Y;
        }


        public void Dispose() {
            foreach(var fontAtlas in _atlasForSize.Values) {
                fontAtlas.Dispose();
            }
        }
    }
}
