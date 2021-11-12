using MinimalAF.Rendering;
using MinimalAF.UI;

namespace MinimalAF.Logic
{
    //Used by all classes associated with program logic
    public abstract class EntryPoint
    {
        //Program lifecycle
        public abstract void Start();

        /// <summary>
        /// Called every frame, at a frequency defined by Window.UpdateFrequency hz
        /// </summary>
        public abstract void Update(double deltaTime);

        /// <summary>
        /// Called every frame, at a frequency defined by Window.RenderFrequency hz
        /// </summary>
        public abstract void Render(double deltaTime);

        /// <summary>
        /// Called whenever the window resizes. 
        /// This should be overridden without calling base.Resize() if you do not want a 2D context
        /// </summary>
        public virtual void Resize() { }

        public virtual void Cleanup() { }
    }
}
