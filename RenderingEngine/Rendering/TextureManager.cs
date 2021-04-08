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
        static Texture _currentTexture = null;

        //Since the current texture can be changed outside of this class even though it shouldnt,
        //this can be used to inform this class of it
        public static void SetCurrentTexture(Texture t)
        {
            _currentTexture = t;
        }

        static Texture _nullTexture = null;

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
            if (_nullTexture != null)
                return;

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

            UseCurrentTexture();
        }

        public void UseCurrentTexture()
        {
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
