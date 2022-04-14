using System;
using System.Drawing;

namespace MinimalAF.Rendering {
    //TODO: implement multiple texture units if needed
    public class TextureManager : IDisposable {
        Texture _currentTexture = null;
        RenderContext ctx;

        //Since the current texture can be changed outside of this class even though it shouldnt,
        //this can be used to inform this class of it
        internal static void SetCurrentTextureChangedFlag() {
            _globalTextureChanged = true;
        }

        static Texture _nullTexture = null;
        static bool _globalTextureChanged = false;

        public static bool GlobalTextureHasChanged() {
            return _globalTextureChanged;
        }

        public Texture Get() {
            return _currentTexture;
        }

        public TextureManager(RenderContext context) {
            InitNullTexture(true);
            ctx = context;
        }

        private void InitNullTexture(bool internalSet=false) {
            Bitmap white1x1 = new Bitmap(1, 1);
            white1x1.SetPixel(0, 0, Color.FromArgb(255, 255, 255, 255));

            _nullTexture = new Texture(white1x1, new TextureImportSettings { });

            if (!internalSet) {
                Set(null);
            }
        }

        public void Set(Texture texture) {
            if (!TextureHasChanged(texture))
                return;

            ctx.Flush();

            _currentTexture = texture;

            UseCurrentTexture();
        }

        public bool TextureHasChanged(Texture texture) {
            return _currentTexture != texture || GlobalTextureHasChanged();
        }

        private void UseCurrentTexture() {
            if (_currentTexture == null) {
                _nullTexture.Use(0);
            } else {
                _currentTexture.Use(0);
            }
        }

        public void Dispose() {
            _nullTexture.Dispose();
        }
    }
}
