using MinimalAF.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI.Components.Debugging
{
    public class UIDebugComponent : UIComponent
    {
        public override UIComponent Copy()
        {
            return new UIDebugComponent();
        }

        public override void OnResize()
        {
            base.OnResize();
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
        }
    }
}
