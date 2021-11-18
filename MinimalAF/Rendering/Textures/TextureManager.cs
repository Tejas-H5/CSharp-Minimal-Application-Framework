using System;
using System.Drawing;

namespace MinimalAF.Rendering
{
    //TODO: implement multiple units if needed
    class TextureManager : IDisposable
    {
        Texture _currentTexture = null;

        //Since the current texture can be changed outside of this class even though it shouldnt,
        //this can be used to inform this class of it
        internal static void SetCurrentTextureChangedFlag()
        {
            _globalTextureChanged = true;
        }

        static Texture _nullTexture = null;
        static bool _globalTextureChanged = false;

        public static bool GlobalTextureHasChanged()
        {
            return _globalTextureChanged;
        }

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

            UseCurrentTexture();
        }

		public bool IsSameTexture(Texture texture)
		{
			return _currentTexture == texture && (!GlobalTextureHasChanged());
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
