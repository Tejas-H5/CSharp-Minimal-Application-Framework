using RenderingEngine.Datatypes;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components
{
    class UILinearArrangement : UIComponent
    {
        private bool _vertical;
        private bool _reverse;
        private float _padding;

        public UILinearArrangement(bool vertical, bool reverse, float padding)
        {
            _vertical = vertical;
            _reverse = reverse;
            _padding = padding;
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

                if (_vertical)
                {
                    _parent[i].RectTransform.SetNormalizedAnchoring(
                        new Rect2D(
                            0,
                            i / (float)_parent.Count,
                            1,
                            (i + 1) / (float)_parent.Count
                            )
                        );

                    _parent[i].SetAbsoluteOffset(new Rect2D(_padding / 2f, _padding / 2f, _padding / 2f, _padding / 2f));
                }
            }
        }

        private void SetChildAnchoring(int i)
        {
            if (_vertical)
            {
                float side = _reverse ? 0 : 1;
                _parent[i].SetNormalizedPositionCenterY(side, side);
            }
            else
            {
                float side = _reverse ? 1 : 0;
                _parent[i].SetNormalizedPositionCenterX(side, side);
            }
        }
    }
}
