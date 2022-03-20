using System;

namespace MinimalAF.VisualTests.UI {
    public class Panel : Element {
        Color4 _color, _hoverColor, _clickColor;
        UIState _uiState;

        public Panel(Color4 color, Color4 hoverColor, Color4 clickColor) {
            _color = color;
            _drawColor = color;
            _hoverColor = hoverColor;
            _clickColor = clickColor;
        }

        Color4 _drawColor;

        public override void OnMount() {
            _uiState = GetResource<UIState>();
        }

        public override void OnUpdate() {
            base.OnUpdate();

            _drawColor = _color;

            if (MouseOverSelf()) {
                if (MouseButtonPressed(MouseButton.Any)) {
                    _drawColor = _clickColor;
                } else {
                    _drawColor = _hoverColor;
                }
            }
        }

        public override void OnRender() {
            SetDrawColor(Color4.VA(0, 1));
            RectOutline(1, RelativeRect);

            SetDrawColor(_drawColor);
            Rect(RelativeRect);

            base.OnRender();
        }
    }
}
