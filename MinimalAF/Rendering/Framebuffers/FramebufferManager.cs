using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace MinimalAF.Rendering {
    public class FramebufferManager : IDisposable {
        private Dictionary<int, Framebuffer> _framebufferList;

        public FramebufferManager() {
            _framebufferList = new Dictionary<int, Framebuffer>();
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
                _framebufferList[index] = new Framebuffer(CTX.ContextWidth, CTX.ContextHeight,
                    new TextureImportSettings {
                        Filtering = FilteringType.NearestNeighbour
                    });
            }


            _framebufferList[index].ResizeIfRequired(CTX.ContextWidth, CTX.ContextHeight);

            CTX.Flush();


            _framebufferList[index].Use();
            CTX.Clear();
        }

        public void UseTransparent(int index) {
            Color4 prevClearColor = CTX.ClearColor;
            CTX.ClearColor = Color4.RGBA(0, 0, 0, 0);

            Use(index);

            CTX.ClearColor = prevClearColor;
        }

        public void StopUsing() {
            CTX.Flush();

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

            CTX.Texture.Set(t);
        }

        /// <summary>
        /// This only works in a 2D context
        /// </summary>
        /// <param name="index"></param>
        public void DrawFramebufferToScreen2D(int index) {
            Texture prevTexture = CTX.Texture.Get();

            CTX.Texture.Set(GetTexture(index));
            CTX.Rect.Draw(0, 0, CTX.ContextWidth, CTX.ContextHeight);

            CTX.Texture.Set(prevTexture);
        }

        /// <summary>
        /// This only works in a 2D context
        /// </summary>
        /// <param name="index"></param>
        public void DrawFramebufferToScreen2D(int index, Rect2D screenRect) {
            Texture prevTexture = CTX.Texture.Get();

            CTX.Texture.Set(GetTexture(index));
            CTX.Rect.Draw(screenRect, new Rect2D(screenRect.X0 / CTX.ContextWidth, screenRect.Y0 / CTX.ContextHeight, screenRect.X1 / CTX.ContextWidth, screenRect.Y1 / CTX.ContextHeight));

            CTX.Texture.Set(prevTexture);
        }

        public void Dispose() {
            foreach (var intFramebufferPair in _framebufferList) {
                intFramebufferPair.Value.Dispose();
            }

            _framebufferList.Clear();
        }
    }
}
