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

        /// <summary>
        /// Arranges the child elements in a linear fashion.
        /// Each element will have the selected axes sized to [elementSizing], and will be 
        /// placed [padding] apart from each other.
        /// 
        /// if an element sizing of -1 or anything less than 0 is specified, then no resizeing will occur
        /// and the child elements will maintain their current size along the selected axis
        /// </summary>
        /// <param name="vertical">vertical or horizontal?</param>
        /// <param name="reverse">forward or reverse?</param>
        /// <param name="elementSizing">how large should each element be in the chosen direction?</param>
        /// <param name="padding">what space should be between them? (can be set negative, but why would you do that)</param>
        public UILinearArrangement(bool vertical, bool reverse, float elementSizing, float padding)
        {
            _vertical = vertical;
            _reverse = reverse;
            _padding = padding;
            _elementSizing = elementSizing;
        }

        public override UIComponent Copy()
        {
            return new UILinearArrangement(_vertical, _reverse, _elementSizing, _padding);
        }

        protected override void OnRectTransformResize(UIRectTransform obj)
        {
            _parent.UpdateRect();
            _parent.ResizeChildren();

            float endSize;
            if(_elementSizing >= 0)
            {
                endSize = _padding + (_elementSizing + _padding) * _parent.Count;
            }
            else
            {
                endSize = _padding;
                for(int i = 0; i < _parent.Count; i++)
                {
                    endSize += GetSizeOfElement(i);
                    endSize += _padding;
                }
            }
            

            if (_vertical)
            {
                _parent.SetAbsPositionSizeY(_parent.AnchoredPositionAbs.Y, endSize);
            }
            else
            {
                _parent.SetAbsPositionSizeX(_parent.AnchoredPositionAbs.Y, endSize);
            }

            float amount = _padding;

            for (int i = 0; i < _parent.Count; i++)
            {
                if(_elementSizing < 0)
                {
                    float size = GetSizeOfElement(i);
                    SetChildAnchoring(i, amount, size);
                    amount += size;
                    amount += _padding;
                }
                else
                {
                    SetChildAnchoring(i, amount, _elementSizing);
                    amount += _elementSizing + _padding;
                }
            }
        }

        private float GetSizeOfElement(int i)
        {
            return _vertical ? _parent[i].Rect.Height : _parent[i].Rect.Width;
        }

        private void SetChildAnchoring(int i, float amount, float size)
        {
            if (_vertical)
            {
                float side = _reverse ? 0 : 1;
                _parent[i].SetNormalizedPositionCenterY(side, side);
                _parent[i].SetAbsPositionSizeY(_reverse ? amount : (-amount), size);
                _parent[i].SetAbsOffsetsX(_padding, _padding);
                _parent[i].SetNormalizedAnchoringX(0, 1);
            }
            else
            {
                float side = _reverse ? 1 : 0;
                _parent[i].SetNormalizedPositionCenterX(side, side);
                _parent[i].SetAbsPositionSizeX(_reverse ? (-amount) : amount, size);
                _parent[i].SetAbsOffsetsY(_padding, _padding);
                _parent[i].SetNormalizedAnchoringY(0, 1);
            }
        }
    }
}
