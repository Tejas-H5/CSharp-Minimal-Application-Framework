using OpenTK.Graphics.OpenGL;
using System;

namespace MinimalAF.Rendering
{
	/// <summary>
	/// Will be used later to implement post-processing, 3D UI, render passes,
	/// transparency in overlapping meshes and more
	/// 
	/// TODO: Make resizeing this more memory efficient
	/// </summary>
	public class Framebuffer : IDisposable
    {
        int _width=-1, _height=-1;

        //int _backingWidth = 0, _backingHeight = 0;

        bool _init = false;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        int _fbo;

        Texture _textureObject;
        TextureImportSettings _textureSettings;

        public Texture Texture {
            get {
                return _textureObject;
            }
        }

        int _renderBufferObject;

        public Framebuffer(int width, int height, TextureImportSettings textureSettings)
        {
            _textureSettings = textureSettings;
            /*
            new TextureImportSettings
            {
                Clamping = ClampingType.ClampToEdge,
                Filtering = FilteringType.NearestNeighbour,
                PixelFormatType = PixelFormat.Rgb,
                InternalFormat = PixelInternalFormat.Rgb
            }*/


            _textureObject = new Texture(1, 1, _textureSettings);

            _fbo = GL.GenFramebuffer();
            _renderBufferObject = GL.GenRenderbuffer();

            ResizeIfRequired(width, height);
        }

        /// <summary>
        /// Reallocates a Texture2D and a renderbuffer under the hood
        /// if the current dimensions are different.
        /// </summary>
        public void ResizeIfRequired(int width, int height)
        {
            if (_width == width && _height == height)
                return;

            _width = width;
            _height = height;

            BindFrameBuffer();


            ReallocateRenderbuffer(_width, _height);

            //TODO: more efficient resizing

            ReallocateTexture(_width, _height);


            ValidateFrameBuffer();

            UnbindFrameBuffer();
        }

        private static void UnbindFrameBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void ReallocateTexture(int width, int height)
        {
            _textureObject.Resize(width, height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _textureObject.Handle, 0);
        }

        private void ReallocateRenderbuffer(int width, int height)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _renderBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, _renderBufferObject);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        private void BindFrameBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        }

        private static void ValidateFrameBuffer()
        {
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("new framebuffer lets go");
            }
            else
            {
                throw new Exception("message to dev: Pls fix");
            }
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
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

        public void Use()
        {
            BindFrameBuffer();
        }

        public void StopUsing()
        {
            UnbindFrameBuffer();
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
