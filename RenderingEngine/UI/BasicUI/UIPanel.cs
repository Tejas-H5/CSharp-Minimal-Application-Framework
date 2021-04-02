using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;

namespace RenderingEngine.UI.BasicUI
{
    public class UIPanel : UINode
    {
        public UIPanel()
        {
            _anchoring = new Rect2D(0, 0,1, 1);
            _rectOffset = new Rect2D(0, 0, 0, 0);
            _backgroundRect = new UIBackgroundRect(this);
        }

        UIBackgroundRect _backgroundRect;
        public UIBackgroundRect BackgroundRect { get { return _backgroundRect; } }


        public override void Update(double deltaTime)
        {
            _isMouseDown = false;
            _isMouseOver = false;

            base.Update(deltaTime);
        }

        bool _isMouseOver;
        bool _isMouseDown;

        public override bool ProcessEvents()
        {
            if (IsMouseOver())
            {
                _isMouseOver = true;
                if (Input.IsAnyClicked)
                {
                    _isMouseDown = true;
                }
                return true;
            }

            return false;
        }

        public override void Draw(double deltaTime)
        {
            base.Draw(deltaTime);

            _backgroundRect.Draw(isMouseOver: _isMouseOver, isMouseDown: _isMouseDown);
        }

    }
}
