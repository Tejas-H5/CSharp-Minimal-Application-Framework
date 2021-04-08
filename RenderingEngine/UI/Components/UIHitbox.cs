using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public abstract class UIHitbox : UIComponent
    {
        public abstract bool PointIsInside(float x, float y);
    }
}
