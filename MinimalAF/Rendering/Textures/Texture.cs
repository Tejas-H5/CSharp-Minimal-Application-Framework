using MinimalAF.ResourceManagement;
using OpenTK.Graphics.OpenGL;
using StbiSharp;
using System;
using System.IO;

namespace MinimalAF.Rendering {
    public struct Image {
        public byte[] Data;
        public int Width;
        public int Height;
        public int NumChannels;

        public Image(int width, int height, int numChannels) {
            Width = width; Height = height; NumChannels = numChannels;
            Data = new byte[width * height * numChannels];
        }
    }

    // Initially Taken from https://github.com/opentk/LearnOpenTK/blob/master/Common/Texture.cs
    // And then modified
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture : IDisposable {
        int _handle;
        Image _image;
        public string path;
        TextureImportSettings _importSettings;

        public int Handle => _handle;
        public int Width => _image.Width;
        public int Height => _image.Height;
        public float AspectRatio => Width / (float)Height;

        public static Texture LoadFromFile(string path, TextureImportSettings settings = null) {
            using var stream = File.OpenRead(path);
            using var memoryStream = new MemoryStream(); 

            stream.CopyTo(memoryStream);
            using StbiImage image = Stbi.LoadFromMemory(memoryStream, 4);

            var ourImage = new Image { 
                Width = image.Width,
                Height = image.Height,
                Data = new byte[image.Data.Length],
                NumChannels = image.NumChannels
            };
            image.Data.CopyTo(ourImage.Data);

            var tex = new Texture(ourImage, settings);
            tex.path = path;
            return tex;
        }


        public Texture(int width, int height, TextureImportSettings settings) {
            _importSettings = settings;
            _handle = GL.GenTexture();
            _importSettings = settings;

            UploadOpenGLImage((IntPtr)null, width, height);
        }


        //TODO: implement this
        internal void Resize(int width, int height) {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            UploadOpenGLImage((IntPtr)null, width, height);
        }

        public Texture(Image image, TextureImportSettings settings = null) {
            if (settings == null) {
                settings = new TextureImportSettings();
            }

            _handle = GL.GenTexture();
            _importSettings = settings;
            _image = image;

            unsafe {
                fixed (byte* data = image.Data) {
                    UploadOpenGLImage((IntPtr)data, image.Width, image.Height);
                }
            }
        }

        private void UploadOpenGLImage(IntPtr data, int width, int height) {
            if (_handle == 0) {
                throw new Exception("We f*kded up");
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            GL.TexImage2D(
                target: TextureTarget.Texture2D,
                level: 0,
                internalformat: _importSettings.InternalFormat,
                width: width,
                height: height,
                border: 0,
                format: _importSettings.PixelFormatType,
                type: PixelType.UnsignedByte,
                pixels: (IntPtr)data
            );

            TextureMinFilter minFilter = _importSettings.GetGLMinFilter();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);

            TextureMagFilter magFilter = _importSettings.GetGLMagFilter();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_importSettings.Clamping);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_importSettings.Clamping);

            TextureManager.SetOpenGLBoundTextureHasInadvertantlyChanged();
        }

        private void BindTextureHandle() {
            
        }

        public void Use(TextureUnit unit) {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To prevent double-frees

        protected virtual void Dispose(bool disposing) {
            if (disposedValue)
                return;

            GLDeletionQueue.QueueTextureForDeletion(Handle);
            disposedValue = true;
        }

        ~Texture() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void UpdateSubImage(int rowPx, int columnPx, Image subImage) {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            unsafe {
                fixed(byte* data = subImage.Data) {
                    GL.TexSubImage2D(
                        TextureTarget.Texture2D, 0, 
                        rowPx, 
                        columnPx,
                        subImage.Width,
                        subImage.Height, 
                        _importSettings.PixelFormatType,
                        PixelType.UnsignedByte,
                        (IntPtr)data
                    );
                }
            }

            TextureManager.SetOpenGLBoundTextureHasInadvertantlyChanged();
        }
        #endregion
    }
}