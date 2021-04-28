using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.Debugging
{
    public class UIDebugComponent : UIComponent
    {
        public override UIComponent Copy()
        {
            return new UIDebugComponent();
        }

        protected override void OnRectTransformResize(UIRectTransform rtf)
        {
            base.OnRectTransformResize(rtf);
        }
    }
}
