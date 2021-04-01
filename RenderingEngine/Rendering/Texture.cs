using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace RenderingEngine.Rendering
{
    // Initially Taken from https://github.com/opentk/LearnOpenTK/blob/master/Common/Texture.cs
    // And then modified
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture : IDisposable
    {
        int _handle;
        private int _height;
        private int _width;

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
                return null;
            }

            return new Texture(image, settings);
        }

        public Texture(Bitmap image, TextureImportSettings settings)
        {
            _width = image.Width;
            _height = image.Height;

            _handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);

            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                image.Width,
                image.Height,
                0,
                PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);


            TextureMinFilter minFilter;
            TextureMagFilter magFilter;

            SetAppropriateFilter(settings, out minFilter, out magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
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
                }

                // Don't forget to dispose of the texture too!
                GL.DeleteTexture(Handle);

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