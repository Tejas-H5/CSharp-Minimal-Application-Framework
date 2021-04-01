using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering
{
    public interface IDrawable
    {
        public void Draw(RenderingContext ctx);
    }
}
