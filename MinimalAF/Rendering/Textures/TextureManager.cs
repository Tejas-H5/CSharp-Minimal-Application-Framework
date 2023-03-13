using System;
using SkiaSharp;

namespace MinimalAF.Rendering {
    //TODO: implement multiple texture units if needed
    public class TextureManager : IDisposable {
        Texture currentTexture = null;

        //Since the current texture can be changed outside of this class even though it shouldnt,
        //this can be used to inform this class of it
        internal static void SetCurrentTextureChangedFlag() {
            globalTextureChanged = true;
        }

        static Texture nullTexture = null;
        static bool globalTextureChanged = false;

        public static bool GlobalTextureHasChanged() {
            return globalTextureChanged;
        }

        public Texture Get() {
            return currentTexture;
        }

        public TextureManager() {
            InitNullTexture();
        }

        private void InitNullTexture() {
            SKBitmap white1x1 = new SKBitmap(1, 1);
            white1x1.SetPixel(0, 0, new SKColor(0xFF, 0xFF, 0xFF, 0xFF));

            nullTexture = new Texture(white1x1, new TextureImportSettings { });

            Use(null);
        }

        public void Use(Texture texture) {
            if (!TextureHasChanged(texture))
                return;

            CTX.Flush();

            currentTexture = texture;

            UseCurrentTexture();
        }

        public bool TextureHasChanged(Texture texture) {
            return currentTexture != texture || GlobalTextureHasChanged();
        }

        private void UseCurrentTexture() {
            if (currentTexture == null) {
                nullTexture.Use(0);
            } else {
                currentTexture.Use(0);
            }
        }

        public void Dispose() {
            nullTexture.Dispose();
        }
    }
}
