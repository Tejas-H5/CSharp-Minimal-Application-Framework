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
    public class UIPanel : UIElement
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

        public override void Update(double deltaTime, GraphicsWindow window)
        {
            base.Update(deltaTime, window);

            if (_children.Count > 0)
                return;

            _isMouseOver = IsMouseOver(window);

            if (_isMouseOver)
                _isMouseDown = window.MouseState.IsAnyButtonDown;
        }


        public override void Draw(double deltaTime, RenderingContext ctx)
        {
            base.Draw(deltaTime, ctx);

            DrawBackgroundRect(ctx);
        }

        private void DrawBackgroundRect(RenderingContext ctx)
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

            ctx.SetDrawColor(color);
            ctx.DrawRect(_rect);
        }

    }
}
