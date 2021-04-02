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
        }

        protected Color4 _color;
        public Color4 Color {
            get => _color;
            set {
                _color = value;
                HoverColor = value;
                ClickedColor = value;
            }
        }

        public Color4 HoverColor { get; set; }
        public Color4 ClickedColor { get; set; }


        protected bool _isMouseOver;
        protected bool _isMouseDown;

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (_children.Count > 0)
                return;

            _isMouseOver = IsMouseOver();

            if (_isMouseOver)
                _isMouseDown = Input.IsAnyClicked;
        }


        public override void Draw(double deltaTime)
        {
            base.Draw(deltaTime);

            DrawBackgroundRect();
        }

        private void DrawBackgroundRect()
        {
            Color4 color;
            if (_isMouseOver)
            {
                if (_isMouseDown)
                {
                    color = ClickedColor;
                }
                else
                {
                    color = HoverColor;
                }
            }
            else
            {
                color = Color;
            }

            if (color.A < 0.0001f)
                return;

            CTX.SetDrawColor(color);
            CTX.DrawRect(_rect);
        }

    }
}
