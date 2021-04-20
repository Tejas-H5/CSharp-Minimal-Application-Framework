using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.AutoResizing
{
    public class UIFitChildren : UIComponent
    {
        bool _horizontal;
        bool _vertical;

        public UIFitChildren(bool horizontal, bool vertical)
        {
            _horizontal = horizontal;
            _vertical = vertical;
        }

        float x0, y0, x1, y1;

        public override void AfterDraw(double deltaTime)
        {
            //CTX.SetDrawColor(1, 0, 1, 0.5f);
            //CTX.DrawRect(x0, y0, x1, y1);
        }

        protected override void OnRectTransformResize(UIRectTransform obj)
        {
            if (!(_horizontal || _vertical))
                return;

            _parent.UpdateRect();
            _parent.ResizeChildren();

            PerformResizeUpdate();
        }

        private void PerformResizeUpdate()
        {
            x0 = Window.Rect.Width;
            y0 = Window.Rect.Height;
            x1 = 0;
            y1 = 0;

            for (int i = 0; i < _parent.Count; i++)
            {
                Rect2D rect = _parent[i].Rect;
                Rect2D anchors = _parent[i].NormalizedAnchoring;
                Rect2D offsets = _parent[i].AbsoluteOffset;

                x0 = MathF.Min(x0, rect.X0);
                y0 = MathF.Min(y0, rect.Y0);

                x1 = MathF.Max(x1, rect.X1);
                y1 = MathF.Max(y1, rect.Y1);
            }


            if (_horizontal && _vertical)
            {
                _parent.Rect = new Rect2D(x0, y0, x1, y1);
            }
            else if (_horizontal)
            {
                Rect2D wantedRect = _parent.Rect;
                wantedRect.X0 = x0;
                wantedRect.X1 = x1;
                _parent.Rect = wantedRect;
            }
            else
            {
                Rect2D wantedRect = _parent.Rect;
                wantedRect.Y0 = y0;
                wantedRect.Y1 = y1;
                _parent.Rect = wantedRect;
            }
        }
    }
}
