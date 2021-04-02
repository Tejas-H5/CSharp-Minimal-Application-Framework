using RenderingEngine.Datatypes;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.BasicUI
{
    public class UIButton : UIElement
    {
        public event Action OnClicked;

        public string Text { get => _text; set => _text = value; }
        public Color4 TextColor { get; set; }

        private string _text;

        UIBackgroundRect _backgroundRect;

        public UIBackgroundRect BackgroundRect { get { return _backgroundRect; } }

        public UIButton()
        {
            _backgroundRect = new UIBackgroundRect(this);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
        }

        public override bool ProcessEvents()
        {
            bool res = base.ProcessEvents();

            if (_isMouseOver)
            {
                if (Input.MouseReleased(MouseButton.Left))
                {
                    OnClicked?.Invoke();
                }
            }

            return res;
        }

        public override void Draw(double deltaTime)
        {
            _backgroundRect.Draw(_isMouseOver, _isMouseDown);

            float textHeight = CTX.GetCharHeight();
            float textWidth = CTX.GetStringWidth(_text);

            CTX.SetDrawColor(TextColor);
            CTX.DrawText(_text, _rect.CenterX - textWidth/2, _rect.CenterY -textHeight/2);
        }

    }
}
