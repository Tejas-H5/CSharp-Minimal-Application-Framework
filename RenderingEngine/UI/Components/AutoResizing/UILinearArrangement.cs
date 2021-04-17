using RenderingEngine.Datatypes;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.AutoResizing
{
    class UILinearArrangement : UIComponent
    {
        private bool _vertical;
        private bool _reverse;
        private float _padding;
        private float _spacing;

        public UILinearArrangement(bool vertical, bool reverse, float spacing, float padding)
        {
            _vertical = vertical;
            _reverse = reverse;
            _padding = padding;
            _spacing = spacing;
        }

        public override void OnResize()
        {
            for (int i = 0; i < _parent.Count; i++)
            {
                _parent[i].UpdateRect();
            }


            for (int i = 0; i < _parent.Count; i++)
            {
                SetChildAnchoring(i);
            }
        }

        private void SetChildAnchoring(int i)
        {
            float amount = _padding + (_spacing + _padding) * i;

            float endSize = _padding + (_spacing + _padding) * _parent.Count;
            if (_vertical)
            {
                _parent.SetAbsPositionSizeY(_parent.AnchoredPositionAbs.Y, endSize);
            }

            if (_vertical)
            {
                float side = _reverse ? 0 : 1;
                _parent[i].SetNormalizedPositionCenterY(side, side);
                _parent[i].SetAbsPositionSizeY(_reverse ? amount : -amount, _spacing);
                _parent[i].SetAbsOffsetsX(_padding, _padding);
                _parent[i].SetNormalizedAnchoringX(0, 1);
            }
            else
            {
                float side = _reverse ? 0 : 1;
                _parent[i].SetNormalizedPositionCenterX(side, side);
                _parent[i].SetAbsPositionSizeX(_reverse ? -amount : amount, _spacing);
                _parent[i].SetAbsOffsetsY(_padding, _padding);
                _parent[i].SetNormalizedAnchoringY(0, 1);
            }

            _parent.UpdateRect();
        }
    }
}
