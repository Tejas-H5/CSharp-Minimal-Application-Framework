using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MinimalAF.Rendering
{
	// Initially Taken from https://github.com/opentk/LearnOpenTK/blob/master/Common/Texture.cs
	// And then modified
	// A helper class, much like Shader, meant to simplify loading textures.
	public class Texture : IDisposable
    {
        int _handle;
        private int _height;
        private int _width;
        TextureImportSettings _importSettings;

        public int Handle {
            get {
                return _handle;
            }
        }

        public int Width {
            get {
                return _width;
            }
        }

        public int Height {
            get {
                return _height;
            }
        }

        public float AspectRatio {
            get {
                return _width / (float)_height;
            }
        }

        //TODO: replace boolean with TextureImportSettings class if needed
        public static Texture LoadFromFile(string path, TextureImportSettings settings)
        {
            Bitmap image;

            try
            {
                image = new Bitmap(path);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw (e);
                //return null;
            }

            return new Texture(image, settings);
        }


        public Texture(int width, int height, TextureImportSettings settings)
        {
            Init(width, height, (IntPtr)null, settings);
        }


        //TODO: implement this
        internal void Resize(int width, int height)
        {
            BindTextureHandle();
            SendImageDataToOpenGL(width, height, (IntPtr)null, _importSettings);
            SendTextureParamsToOpenGL(_importSettings);

            TextureManager.SetCurrentTextureChangedFlag();
        }

        public Texture(Bitmap image, TextureImportSettings settings)
        {
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            Init(image.Width, image.Height, data.Scan0, settings);

            image.UnlockBits(data);
        }

        private void Init(int width, int height, IntPtr data, TextureImportSettings settings)
        {
            _handle = GL.GenTexture();
            _importSettings = settings;

            BindTextureHandle();
            SendImageDataToOpenGL(width, height, data, settings);
            SendTextureParamsToOpenGL(settings);

            TextureManager.SetCurrentTextureChangedFlag();
        }

        private void BindTextureHandle()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }


        private void SendImageDataToOpenGL(int width, int height, IntPtr data, TextureImportSettings settings)
        {
            _width = width;
            _height = height;

            GL.TexImage2D(TextureTarget.Texture2D,
                            0,
                            settings.InternalFormat,
                            width,
                            height,
                            0,
                            settings.PixelFormatType,
                            PixelType.UnsignedByte,
                            data);
        }

        private static void SendTextureParamsToOpenGL(TextureImportSettings settings)
        {
            TextureMinFilter minFilter;
            TextureMagFilter magFilter;

            SetAppropriateFilter(settings, out minFilter, out magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)settings.Clamping);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)settings.Clamping);
        }


        private static void SetAppropriateFilter(TextureImportSettings settings, out TextureMinFilter minFilter, out TextureMagFilter magFilter)
        {
            switch (settings.Filtering)
            {
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

        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Don't forget to dispose of the texture too!
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.DeleteTexture(Handle);
                }


                Console.WriteLine("Texture destructed");

                disposedValue = true;
            }
        }

        ~Texture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}