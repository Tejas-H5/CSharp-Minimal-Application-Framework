using MinimalAF.Rendering.Text;
using System;
using System.Drawing;

namespace MinimalAF.Rendering.ImmediateMode {
    public class TextDrawer : IDisposable {
        FontManager _fontManager;

        public FontAtlasTexture ActiveFont {
            get {
                return _fontManager.ActiveFont;
            }
        }

        public TextDrawer() {
            _fontManager = new FontManager();
            SetFont("", -1);
        }


        public void SetFont(string name, int size) {
            if (size < 0)
                size = 12;

            _fontManager.SetCurrentFont(name, size);
        }


        public float CharWidth {
            get {
                return ActiveFont.FontAtlas.CharWidth;
            }
        }

        public float CharHeight {
            get {
                return ActiveFont.FontAtlas.CharHeight;
            }
        }

        public float GetWidth() {
            return GetWidth(' ');
        }

        public float GetHeight() {
            return GetHeight('|');
        }


        public float GetWidth(char c) {
            if (ActiveFont.FontAtlas.IsValidCharacter(c)) {
                return ActiveFont.FontAtlas.GetCharacterSize(c).Width;
            }


            float spaceWidth = ActiveFont.FontAtlas.GetCharacterSize('|').Width;

            switch (c) {
                case ' ':
                    return spaceWidth;
                case '\t':
                    return 4 * spaceWidth;
            }

            return 0;
        }

        public float GetHeight(char c) {
            return ActiveFont.FontAtlas.GetCharacterSize(c).Height;
        }

        public SizeF GetSize(char c) {
            return ActiveFont.FontAtlas.GetCharacterSize(c);
        }


        public Texture UnderlyingFontAtlas {
            get {
                return ActiveFont.FontTexture;
            }
        }

        private float CaratPosX(float lineWidth, HorizontalAlignment hAlign) {
            switch (hAlign) {
                case HorizontalAlignment.Center:
                    return -lineWidth / 2f;
                case HorizontalAlignment.Right:
                    return -lineWidth;
                default:
                    return 0;
            }
        }

        //TODO: IMPLEMENT tabs and newlines
        //And vertical/horizontal aiignment features
        public PointF Draw(string text, float startX, float startY, HorizontalAlignment hAlign, VerticalAlignment vAlign, float scale = 1.0f) {
            PointF caratPos = new PointF(startX, startY);

            if (text == null)
                return caratPos;

            float textHeight = scale * GetStringHeight(text);
            float charHeight = scale * GetHeight('|');


            switch (vAlign) {
                case VerticalAlignment.Bottom:
                    caratPos.Y = startY + textHeight - charHeight;
                    break;
                case VerticalAlignment.Center:
                    caratPos.Y = startY + textHeight / 2f - charHeight;
                    break;
                case VerticalAlignment.Top:
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

                caratPos = Draw(text, lineStart, lineEnd, caratPos.X, caratPos.Y, scale);

                lineStart = lineEnd;
            }

            return caratPos;
        }

        public PointF Draw(string text, float startX, float startY, float scale = 1.0f) {
            return Draw(text, 0, text.Length, startX, startY, scale);
        }

        public PointF Draw(string text, int start, int end, float startX, float startY, float scale) {
            Texture previousTexture = CTX.Texture.Get();

            CTX.Texture.Set(ActiveFont.FontTexture);

            float x = startX;
            float y = startY;

            for (int i = start; i < end; i++) {
                char c = text[i];

                if (ActiveFont.FontAtlas.IsValidCharacter(c)) {
                    DrawCharacter(scale, x, y, c);
                } else {
                    if (c == '\n') {
                        x = startX;
                        y -= ActiveFont.FontAtlas.CharHeight + 2;
                    }
                }

                x += GetWidth(c);
            }

            CTX.Texture.Set(previousTexture);

            return new PointF(x, y);
        }

        private void DrawCharacter(float scale, float x, float y, char c) {
            SizeF size = GetSize(c);
            Rect uv = ActiveFont.FontAtlas.GetCharacterUV(c);

            CTX.Rect.Draw(new Rect(x, y, x + size.Width * scale, y + size.Height * scale), uv);
        }

        public void Dispose() {
            _fontManager.Dispose();
        }

        public float GetStringHeight(string s) {
            return GetStringHeight(s, 0, s.Length);
        }

        public float GetStringHeight(string s, int start, int end) {
            return _fontManager.GetStringHeight(s, start, end);
        }


        public float GetStringWidth(string s) {
            return GetStringWidth(s, 0, s.Length);
        }

        public float GetStringWidth(string s, int start, int end) {
            return _fontManager.GetStringWidth(s, start, end);
        }
    }
}

