using System;

namespace MinimalAF.VisualTests.UI {
    public class Panel : Element {
        Color4 _color, _hoverColor, _clickColor;

        public Panel(Color4 color, Color4 hoverColor, Color4 clickColor) {
            _color = color;
            _drawColor = color;

            _hoverColor = hoverColor;
            _clickColor = clickColor;
        }

        Color4 _drawColor;
        WindowMouseInput _mouseInput;

        public override void OnStart() {
            _mouseInput = GetAncestor<Window>().MouseInput;

            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();

            _drawColor = _color;

            if (_mouseInput.CheckAndHandleOver(this)) {
                if (_mouseInput.CheckAndHandlePressed()) {
                    Console.WriteLine("Clicked");
                    _drawColor = _clickColor;
                } else {
                    _drawColor = _hoverColor;
                }
            }
        }

        public override void OnRender() {
            SetDrawColor(new Color4(0, 1));
            RectOutline(1, Rect);

            SetDrawColor(_drawColor);
            Rect(Rect);

            base.OnRender();
        }
    }
}
