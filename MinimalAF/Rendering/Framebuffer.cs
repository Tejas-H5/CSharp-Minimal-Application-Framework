using OpenTK.Graphics.OpenGL;
using System;

namespace MinimalAF.Rendering.Uncomplete
{
    /// <summary>
    /// Will be used later to implement post-processing and 3D UI
    /// </summary>
    public class Framebuffer : IDisposable
    {
        int _width, _height;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        int _fbo;

        Texture _textureObject;
        int _renderBufferObject;

        public Framebuffer(int width, int height)
        {
            _width = width;
            _height = height;

            _fbo = GL.GenFramebuffer();

            _textureObject = new Texture(1, 1,
                new TextureImportSettings
                {
                    Clamping = ClampingType.ClampToEdge,
                    Filtering = FilteringType.NearestNeighbour,
                    PixelFormatType = PixelFormat.Rgb,
                    InternalFormat = PixelInternalFormat.Rgb
                });

            _renderBufferObject = GL.GenRenderbuffer();


            Resize(width, height);
        }

        /// <summary>
        /// Reallocates a Texture2D and a renderbuffer under the hood
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            _textureObject.Resize(width, height);

            //Resize renderbuffer
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _textureObject.Handle, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _renderBufferObject);
        }


        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                _textureObject.Dispose();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Framebuffer()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
