using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Text;

namespace RenderingEngine.Rendering
{
    //TODO: implement multiple units if needed
    class TextureManager : IDisposable
    {
        Texture _currentTexture = null;

        Texture _nullTexture;

        public Texture CurrentTexture()
        {
            return _currentTexture;
        }

        public TextureManager()
        {
            InitNullTexture();
        }

        private void InitNullTexture()
        {
            Bitmap white1x1 = new Bitmap(1, 1);
            white1x1.SetPixel(0, 0, Color.FromArgb(255, 255, 255, 255));

            _nullTexture = new Texture(white1x1, new TextureImportSettings { });

            SetTexture(null);
        }

        public bool IsCurrentTexture(Texture texture)
        {
            return (_currentTexture == texture);
        }

        public void SetTexture(Texture texture)
        {
            _currentTexture = texture;

            if (_currentTexture == null)
            {
                _nullTexture.Use(0);
            }
            else
            {
                _currentTexture.Use(0);
            }
        }

        public void Dispose()
        {
            _nullTexture.Dispose();
        }
    }
}
