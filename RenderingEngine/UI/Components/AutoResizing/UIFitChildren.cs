using RenderingEngine.Datatypes;
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
        Rect2D _rect;

        private Rect2D GetWantedRect()
        {
            Rect2D wantedRect = _parent.Rect;


            if (_horizontal && _vertical)
            {
                wantedRect = new Rect2D(x0, y0, x1, y1);
            }
            else if (_horizontal)
            {
                wantedRect.X0 = x0;
                wantedRect.X1 = x1;
            }
            else
            {
                wantedRect.Y0 = y0;
                wantedRect.Y1 = y1;
            }

            return wantedRect;
        }

        private void ExpandRectToFitChld(int i)
        {
            Rect2D rect = _parent[i].Rect;

            x0 = MathF.Min(x0, rect.X0);
            y0 = MathF.Min(y0, rect.Y0);

            x1 = MathF.Max(x1, rect.X1);
            y1 = MathF.Max(y1, rect.Y1);
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
                ExpandRectToFitChld(i);
            }

            _parent.Rect = GetWantedRect();
        }
    }
}
