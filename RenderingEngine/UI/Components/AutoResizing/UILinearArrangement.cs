using MinimalAF.Datatypes;
using MinimalAF.UI.Core;

namespace MinimalAF.UI.Components.AutoResizing
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

        public override void OnResize()
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
                    if (!_parent[i].IsVisible)
                        continue;

                    endSize += GetSizeOfElement(i);
                    endSize += _padding;
                }
            }
            

            if (_vertical)
            {
                _parent.PosSizeY(_parent.AnchoredPositionAbs.Y, endSize);
            }
            else
            {
                _parent.PosSizeX(_parent.AnchoredPositionAbs.X, endSize);
            }

            float amount = _padding;

            for (int i = 0; i < _parent.Count; i++)
            {
                if (!_parent[i].IsVisible)
                    continue;

                if (_elementSizing < 0)
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
                _parent[i]
                    .AnchoredPosCenterY(side, side)
                    .PosSizeY(y: _reverse ? amount : (-amount), height: size)
                    .OffsetsX(left: _padding, right: _padding)
                    .AnchorsX(0, 1);
            }
            else
            {
                float side = _reverse ? 1 : 0;
                _parent[i]
                    .AnchoredPosCenterX(side,side)
                    .PosSizeX(x: _reverse ? -amount : amount, width: size)
                    .OffsetsY(bottom: _padding, top: _padding)
                    .AnchorsY(0, 1);
            }
        }
    }
}
