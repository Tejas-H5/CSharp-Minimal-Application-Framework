using OpenTK.Graphics.OpenGL;
using System;

namespace MinimalAF.Rendering {
    public class FramebufferManager : IDisposable {
        public Framebuffer current;
        public Framebuffer Current => current;

        public void Use(Framebuffer framebuffer) {
            CTX.Flush();

            if (framebuffer == null) {
                StopUsing();
                return;
            }


            framebuffer.Use();

            CTX.SetViewport(new Rect(0, 0, framebuffer.Width, framebuffer.Height));

            CTX.ContextWidth = framebuffer.Width;
            CTX.ContextHeight = framebuffer.Height;

            current = framebuffer;
        }

        private void StopUsing() {
            CTX.ContextWidth = CTX.ScreenWidth;
            CTX.ContextHeight = CTX.ScreenHeight;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            CTX.SetViewport(new Rect(0, 0, CTX.ContextWidth, CTX.ContextHeight));

            current = null;
        }

        public void Dispose() {
            // was used before but now it's a no-op
        }
    }
}
