using RenderingEngine.Datatypes;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.AutoResizing
{
    public class UILinearArrangement : UIComponent
    {
        private bool _vertical;
        private bool _reverse;
        private float _padding;
        private float _elementSizing;

        public UILinearArrangement(bool vertical, bool reverse, float elementSizing, float padding)
        {
            _vertical = vertical;
            _reverse = reverse;
            _padding = padding;
            _elementSizing = elementSizing;
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);
        }

        protected override void OnRectTransformResize(UIRectTransform obj)
        {
            _parent.UpdateRect();
            _parent.ResizeChildren();

            float endSize = _padding + (_elementSizing + _padding) * _parent.Count;
            if (_vertical)
            {
                _parent.SetAbsPositionSizeY(_parent.AnchoredPositionAbs.Y, endSize);
            }
            else
            {
                _parent.SetAbsPositionSizeX(_parent.AnchoredPositionAbs.Y, endSize);
            }

            for (int i = 0; i < _parent.Count; i++)
            {
                SetChildAnchoring(i, endSize);
            }
        }

        private void SetChildAnchoring(int i, float endSize)
        {
            float amount = _padding + (_elementSizing + _padding) * i;

            if (_vertical)
            {
                float side = _reverse ? 0 : 1;
                _parent[i].SetNormalizedPositionCenterY(side, side);
                _parent[i].SetAbsPositionSizeY(_reverse ? amount : (-amount), _elementSizing);
                _parent[i].SetAbsOffsetsX(_padding, _padding);
                _parent[i].SetNormalizedAnchoringX(0, 1);
            }
            else
            {
                float side = _reverse ? 1 : 0;
                _parent[i].SetNormalizedPositionCenterX(side, side);
                _parent[i].SetAbsPositionSizeX(_reverse ? (-amount) : amount, _elementSizing);
                _parent[i].SetAbsOffsetsY(_padding, _padding);
                _parent[i].SetNormalizedAnchoringY(0, 1);
            }
        }
    }
}
