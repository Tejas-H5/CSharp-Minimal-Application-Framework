using MinimalAF.Rendering;

namespace MinimalAF.Logic
{
    //Used by all classes associated with program logic
    public abstract class EntryPoint
    {
        //Program lifecycle
        public abstract void Start();
        public abstract void Update(double deltaTime);
        public abstract void Render(double deltaTime);

        /// <summary>
        /// Called whenever the window resizes. 
        /// This should be overridden without calling base.Resize() if you do not want a 2D context
        /// </summary>
        public virtual void Resize()
        {
            CTX.Viewport2D(Window.Width, Window.Height);
        }

        public virtual void Cleanup(){}
    }
}
