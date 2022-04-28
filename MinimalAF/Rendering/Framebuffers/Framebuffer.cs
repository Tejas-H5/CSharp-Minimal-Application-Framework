using OpenTK.Graphics.OpenGL;
using System;

namespace MinimalAF.Rendering {
    /// <summary>
    /// Will be used later to implement post-processing, 3D UI, render passes,
    /// transparency in overlapping meshes and more
    /// 
    /// TODO: Make resizeing this more memory efficient
    /// </summary>
    public class Framebuffer : IDisposable {
        int width = -1, height = -1;
        int fbo;
        int renderBufferObject;
        Texture textureObject;
        TextureImportSettings textureSettings;

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


        public Texture Texture {
            get {
                return textureObject;
            }
        }


        public Framebuffer(int width, int height, TextureImportSettings textureSettings = null) {
            if(textureSettings == null) {
                textureSettings = new TextureImportSettings();
            }

            this.textureSettings = textureSettings;

            textureObject = new Texture(1, 1, textureSettings);
            fbo = GL.GenFramebuffer();
            renderBufferObject = GL.GenRenderbuffer();

            Resize(width, height);
        }

        /// <summary>
        /// Reallocates a Texture2D and a renderbuffer under the hood
        /// if the current dimensions are different.
        /// </summary>
        public void Resize(int width, int height) {
            if (this.width == width && this.height == height)
                return;

            this.width = width;
            this.height = height;

            BindFrameBuffer();

            ReallocateRenderbuffer(width, height);

            //TODO: more efficient resizing
            ReallocateTexture(width, height);

            ValidateFrameBuffer();


            UnbindFrameBuffer();
        }

        private static void UnbindFrameBuffer() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void ReallocateTexture(int width, int height) {
            textureObject.Resize(width, height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, textureObject.Handle, 0);
        }

        private void ReallocateRenderbuffer(int width, int height) {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, renderBufferObject);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        private void BindFrameBuffer() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
        }

        private static void ValidateFrameBuffer() {
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferComplete) {
                Console.WriteLine("new framebuffer lets go");
            } else {
                throw new Exception("message to dev: Pls fix");
            }
        }

        internal void Use() {
            BindFrameBuffer();
        }

        internal void StopUsing() {
            UnbindFrameBuffer();
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue)
            {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                textureObject.Dispose();

                disposedValue = true;
            }
        }

        ~Framebuffer() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
