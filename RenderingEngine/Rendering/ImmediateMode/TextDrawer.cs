using RenderingEngine.Datatypes;
using RenderingEngine.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RenderingEngine.Rendering.ImmediateMode
{
    class TextDrawer : IDisposable
    {
        QuadDrawer _quadDrawer;

        FontManager _fontManager;
        internal FontAtlasTexture ActiveFont {
            get { return _fontManager.ActiveFont; }
        }


        public TextDrawer(QuadDrawer quadDrawer)
        {
            _quadDrawer = quadDrawer;

            _fontManager = new FontManager();
            SetCurrentFont("", -1);
        }


        public void SetCurrentFont(string name, int size)
        {
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

        public float GetCharWidth(char c)
        {
            if (ActiveFont.FontAtlas.IsValidCharacter(c))
            {
                return ActiveFont.FontAtlas.GetCharacterSize(c).Width;
            }


            float spaceWidth = ActiveFont.FontAtlas.GetCharacterSize('|').Width;

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
            return ActiveFont.FontAtlas.GetCharacterSize(c).Height;
        }

        public SizeF GetCharSize(char c)
        {
            return ActiveFont.FontAtlas.GetCharacterSize(c);
        }


        public Texture UnderlyingFontAtlas {
            get {
                return ActiveFont.FontTexture;
            }
        }


        private Rect2D GetAtlasRect(char c)
        {
            return new Rect2D(0, 0, 0, 0);
        }


        public PointF DrawText(string text, float startX, float startY, float scale = 1.0f)
        {
            return DrawText(text, 0, text.Length, startX, startY, scale);
        }

        //TODO: IMPLEMENT tabs and newlines
        //And vertical/horizontal aiignment features
        public PointF DrawText(string text, int start, int end, float startX, float startY, float scale = 1.0f)
        {
            CTX.SetTexture(ActiveFont.FontTexture);

            float x = startX;
            float y = startY;

            for (int i = start; i < end; i++)
            {
                char c = text[i];

                if (ActiveFont.FontAtlas.IsValidCharacter(c))
                {
                    DrawCharacter(scale, x, y, c);
                }
                else
                {
                    if (c == '\n')
                    {
                        x = startX;
                        y -= ActiveFont.FontAtlas.CharHeight + 2;
                    }
                }

                x += GetCharWidth(c);
            }

            return new PointF(x, y);
        }

        private void DrawCharacter(float scale, float x, float y, char c)
        {
            SizeF size = GetCharSize(c);
            Rect2D uv = ActiveFont.FontAtlas.GetCharacterUV(c);

            _quadDrawer.DrawRect(new Rect2D(x, y, x + size.Width * scale, y + size.Height * scale), uv);
        }

        public void Dispose()
        {
            _fontManager.Dispose();
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
    }
}

