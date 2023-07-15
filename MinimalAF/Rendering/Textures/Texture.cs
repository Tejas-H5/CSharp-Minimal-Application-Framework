﻿using MinimalAF.ResourceManagement;
using OpenTK.Graphics.OpenGL;
using System;
using SkiaSharp;
using System.IO;

namespace MinimalAF.Rendering {
    // Initially Taken from https://github.com/opentk/LearnOpenTK/blob/master/Common/Texture.cs
    // And then modified
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture : IDisposable {
        int _handle;
        private int height;
        private int width;
        public string path;
        TextureImportSettings _importSettings;

        public int Handle {
            get {
                return _handle;
            }
        }

        public int Width {
            get {
                return width;
            }
        }

        public int Height {
            get {
                return height;
            }
        }

        public float AspectRatio {
            get {
                return width / (float)height;
            }
        }

        public static Texture LoadFromFile(string path, TextureImportSettings settings = null) {
            SKBitmap image;

            try {
                var bytes = File.ReadAllBytes(path);

                var skImportSettings = new SKImageInfo {
                    AlphaType = SKAlphaType.Unpremul
                };

                image = SKBitmap.Decode(bytes);
            } catch (Exception e) {
                Console.Write(e.Message);
                throw;
            }

            var tex = new Texture(image, settings);
            tex.path = path;
            return tex;
        }


        public Texture(int width, int height, TextureImportSettings settings) {
            Init(width, height, (IntPtr)null, settings);
        }


        //TODO: implement this
        internal void Resize(int width, int height) {
            BindTextureHandle();
            SendImageDataToOpenGL(width, height, (IntPtr)null, _importSettings);
            SendTextureParamsToOpenGL(_importSettings);

            TextureManager.SetOpenGLBoundTextureHasInadvertantlyChanged();
        }

        public Texture(SKBitmap image, TextureImportSettings settings = null) {
            if (settings == null) {
                settings = new TextureImportSettings();
            }

            var addr = image.GetPixels();

            Init(image.Width, image.Height, addr, settings);
        }

        private void Init(int width, int height, IntPtr data, TextureImportSettings settings) {
            _handle = GL.GenTexture();
            _importSettings = settings;

            BindTextureHandle();
            SendImageDataToOpenGL(width, height, data, settings);
            SendTextureParamsToOpenGL(settings);

            TextureManager.SetOpenGLBoundTextureHasInadvertantlyChanged();
        }

        private void BindTextureHandle() {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }


        private void SendImageDataToOpenGL(int width, int height, IntPtr data, TextureImportSettings settings) {
            this.width = width;
            this.height = height;

            GL.TexImage2D(
                target:         TextureTarget.Texture2D,
                level:          0,
                internalformat: settings.InternalFormat,
                width:          width,
                height:         height,
                border:         0,
                format:         settings.PixelFormatType,
                type:           PixelType.UnsignedByte,
                pixels:         data
            );
        }

        private static void SendTextureParamsToOpenGL(TextureImportSettings settings) {
            TextureMinFilter minFilter;
            TextureMagFilter magFilter;

            SetAppropriateFilter(settings, out minFilter, out magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)settings.Clamping);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)settings.Clamping);
        }


        private static void SetAppropriateFilter(TextureImportSettings settings, out TextureMinFilter minFilter, out TextureMagFilter magFilter) {
            switch (settings.Filtering) {
                case FilteringType.NearestNeighbour:
                    minFilter = TextureMinFilter.Nearest;
                    magFilter = TextureMagFilter.Nearest;
                    break;
                default:
                    minFilter = TextureMinFilter.Linear;
                    magFilter = TextureMagFilter.Linear;
                    break;
            }
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

        internal void UpdateSubImage(int rowPx, int columnPx, SKBitmap subImage) {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexSubImage2D(
                TextureTarget.Texture2D, 0, 
                rowPx, 
                columnPx,
                subImage.Width,
                subImage.Height, 
                _importSettings.PixelFormatType,
                PixelType.UnsignedByte,
                subImage.GetPixels()
            );

            TextureManager.SetOpenGLBoundTextureHasInadvertantlyChanged();
        }
        #endregion
    }
}