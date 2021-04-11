using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UIMultiEdgeSnapConstraint : UIComponent
    {
        UIEdgeSnapConstraint[] _edgeSnapConstraints;

        public UIMultiEdgeSnapConstraint(params UIEdgeSnapConstraint[] edgeSnapConstraints)
        {
            _edgeSnapConstraints = edgeSnapConstraints;
        }

        public override void SetParent(UIElement parent)
        {
            for (int i = 0; i < _edgeSnapConstraints.Length; i++)
            {
                _edgeSnapConstraints[i].SetParent(parent);
            }
        }

        public override void OnResize()
        {
            for(int i = 0; i < _edgeSnapConstraints.Length; i++)
            {
                _edgeSnapConstraints[i].OnResize();
            }
        }
    }
}
