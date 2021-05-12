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
        bool _debug;

        //TODO: Fix bug where if margin is not equal to the rectOffsets of all the children, it doesn't resize properly
        public UIFitChildren(bool horizontal, bool vertical, bool debug = false)
        {
            _horizontal = horizontal;
            _vertical = vertical;
            _debug = debug;
        }

        float x0, y0, x1, y1;

        //visuialize resizeing
        double timer = 0;
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (!_debug)
                return;

            timer += deltaTime;

            if(timer > 1)
            {
                timer = 0;
                Tick();
            }
        }

        int childIndex = -1;

        private void Tick()
        {
            childIndex++;
            if (childIndex >= _parent.Count)
                childIndex = -1;

            if (childIndex == -1)
            {
                x0 = _parent.Rect.X0;
                y0 = _parent.Rect.Y0;
                x1 = _parent.Rect.X1;
                y1 = _parent.Rect.Y1;
                return;
            }

            ExpandRectToFitChld(childIndex);
        }


        public override void AfterDraw(double deltaTime)
        {
            if (_debug)
            {
                CTX.SetDrawColor(0, 0, 1, 0.5f);
                CTX.DrawRect(x0, y0, x1, y1);
            }
        }


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
            if (!_parent[i].IsVisible)
                return;

            Rect2D rect = _parent[i].Rect;

            x0 = MathF.Min(x0, rect.X0);
            y0 = MathF.Min(y0, rect.Y0);

            x1 = MathF.Max(x1, rect.X1);
            y1 = MathF.Max(y1, rect.Y1);
        }

        public override void OnResize()
        {
            if (!(_horizontal || _vertical))
                return;

            _parent.UpdateRect();
            _parent.ResizeChildren();

            UpdateWantedRect();
            var wantedRect = GetWantedRect();

            ///*

            if (_vertical && _horizontal)
            {
                _parent.PosSize(_parent.AnchoredPositionAbs.X, _parent.AnchoredPositionAbs.X,
                    wantedRect.Width, wantedRect.Height);
            }
            else if(_vertical)
            {
                _parent.PosSizeY(_parent.AnchoredPositionAbs.Y, wantedRect.Height);
            }
            else
            {
                _parent.PosSizeX(_parent.AnchoredPositionAbs.X + wantedRect.Height, wantedRect.Width);
            }
            //*/

            //_parent.Rect = wantedRect;
        }

        private void UpdateWantedRect()
        {
            x0 = _parent.Rect.X1;
            y0 = _parent.Rect.Y1;
            x1 = _parent.Rect.X0;
            y1 = _parent.Rect.Y0;

            for (int i = 0; i < _parent.Count; i++)
            {
                ExpandRectToFitChld(i);
            }
        }

        public override UIComponent Copy()
        {
            return new UIFitChildren(_horizontal, _vertical, _debug);
        }
    }
}
