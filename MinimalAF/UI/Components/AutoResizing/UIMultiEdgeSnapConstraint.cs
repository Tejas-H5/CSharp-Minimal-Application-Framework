using MinimalAF.UI;

namespace MinimalAF.UI
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

        public override UIComponent Copy()
        {
            var copy = new UIEdgeSnapConstraint[_edgeSnapConstraints.Length];
            for(int i =0; i < copy.Length; i++)
            {
                copy[i] = (UIEdgeSnapConstraint)_edgeSnapConstraints[i].Copy();
            }

            return new UIMultiEdgeSnapConstraint(copy);
        }
    }
}
