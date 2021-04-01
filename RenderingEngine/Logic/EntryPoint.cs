using OpenTK.Windowing.Common;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Logic
{
    //Used by all classes associated with program logic
    public abstract class EntryPoint
    {
        protected RenderingContext ctx;
        protected GraphicsWindow window;
        //Program lifecycle
        public virtual void Start(RenderingContext ctx, GraphicsWindow window)
        {
            this.ctx = ctx;
            this.window = window;
        }

        public abstract void Update(double deltaTime);
        public abstract void Render(double deltaTime);


        /// <summary>
        /// Called whenever the window resizes. 
        /// This should be overridden without calling base.Resize() if you do not want a 2D context
        /// </summary>
        public virtual void Resize()
        {
            ctx.Viewport2D(window.Width, window.Height);
        }
        

        public virtual void Cleanup()
        {
            ctx.Dispose();
        }

        /*
        //User Input
        public void OnMouseClick(GraphicsWindow window);
        public void OnMouseRelease(GraphicsWindow window);
        public void OnMouseHold(GraphicsWindow window);
        public void OnMouseWheel(GraphicsWindow window, float offset);
        public void OnMouseMove(GraphicsWindow window);

        public void OnKeyPress(GraphicsWindow window);
        public void OnKeyRelease(GraphicsWindow window);
        public void OnKeyHold(GraphicsWindow window);
        */
    }
}
