using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace MinimalAF.Rendering {
    public class FramebufferManager : IDisposable {
        public void Use(Framebuffer framebuffer) {
            CTX.Flush();

            framebuffer.Use();
            CTX.SetViewport(new Rect(0, 0, framebuffer.Width, framebuffer.Height));

            CTX.ContextWidth = framebuffer.Width;
            CTX.ContextHeight = framebuffer.Height;
        }

        public void StopUsing() {
            CTX.Flush();

            CTX.ContextWidth = CTX.ScreenWidth;
            CTX.ContextHeight = CTX.ScreenHeight;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            CTX.SetViewport(new Rect(0, 0, CTX.ContextWidth, CTX.ContextHeight));
        }


        public void Dispose() {
            // was used before but now it's a no-op
        }
    }
}
