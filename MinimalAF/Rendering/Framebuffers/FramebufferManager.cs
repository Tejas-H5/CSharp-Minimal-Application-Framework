using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace MinimalAF.Rendering {
    internal class FramebufferManager : IDisposable {
        private Dictionary<int, Framebuffer> _framebufferList;
        RenderContext ctx;

        internal FramebufferManager(RenderContext context) {
            _framebufferList = new Dictionary<int, Framebuffer>();
            ctx = context;
        }


        /// <summary>
        /// Creates a new framebuffer with a depth buffer and a stencil buffer for the given index
        /// that is the same size as the window (Specified with Viewport2D (or Viewport3D when it exists) )
        /// if it does not yet exist, and tells the rendering API to start using it.
        /// 
        /// If you are using MinimalAF, the window size thing should be taken care of automatically.
        /// As of now, drawing to a framebuffer of any size other than the window size does not work
        /// </summary>
        public void Use(int index) {
            if (!_framebufferList.ContainsKey(index)) {
                _framebufferList[index] = new Framebuffer(ctx.ContextWidth, ctx.ContextHeight,
                    new TextureImportSettings {
                        Filtering = FilteringType.NearestNeighbour
                    });
            }


            _framebufferList[index].ResizeIfRequired(ctx.ContextWidth, ctx.ContextHeight);

            ctx.Flush();


            _framebufferList[index].Use();
            ctx.Clear();
        }

        public void UseTransparent(int index) {
            Color4 prevClearColor = ctx.ClearColor;
            ctx.ClearColor = Color4.RGBA(0, 0, 0, 0);

            Use(index);

            ctx.ClearColor = prevClearColor;
        }

        public void StopUsing() {
            ctx.Flush();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public Texture GetTexture(int framebufferIndex) {
            if (!_framebufferList.ContainsKey(framebufferIndex)) {
                return null;
            }

            return _framebufferList[framebufferIndex].Texture;
        }

        public void UseTexture(int framebufferIndex) {
            Texture t = GetTexture(framebufferIndex);
            if (t == null)
                return;

            ctx.Texture.Set(t);
        }

        /// <summary>
        /// This only works in a 2D context
        /// </summary>
        /// <param name="index"></param>
        public void DrawFramebufferToScreen2D(int index) {
            Texture prevTexture = ctx.Texture.Get();

            ctx.Texture.Set(GetTexture(index));
            ctx.Rect.Draw(0, 0, ctx.ContextWidth, ctx.ContextHeight);

            ctx.Texture.Set(prevTexture);
        }

        /// <summary>
        /// This only works in a 2D context
        /// </summary>
        /// <param name="index"></param>
        public void DrawFramebufferToScreen2D(int index, Rect screenRect) {
            Texture prevTexture = ctx.Texture.Get();

            ctx.Texture.Set(GetTexture(index));
            ctx.Rect.Draw(screenRect, new Rect(screenRect.X0 / ctx.ContextWidth, screenRect.Y0 / ctx.ContextHeight, screenRect.X1 / ctx.ContextWidth, screenRect.Y1 / ctx.ContextHeight));

            ctx.Texture.Set(prevTexture);
        }

        public void Dispose() {
            foreach (var intFramebufferPair in _framebufferList) {
                intFramebufferPair.Value.Dispose();
            }

            _framebufferList.Clear();
        }
    }
}
