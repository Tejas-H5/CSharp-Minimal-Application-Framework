using System;

namespace MinimalAF.VisualTests.UI {
    public class Panel : Element {
        Color4 color, hoverColor, clickColor;
        UIState uiState;

        public Panel(Color4 color, Color4 hoverColor, Color4 clickColor) {
            this.color = color;
            drawColor = color;
            this.hoverColor = hoverColor;
            this.clickColor = clickColor;
        }

        Color4 drawColor;

        public override void OnMount(Window w) {
            uiState = GetResource<UIState>();
        }

        public override void AfterUpdate() {
            drawColor = color;

            if (uiState.EventWasHandled)
                return;

            if (MouseOverSelf) {
                uiState.EventWasHandled = true;

                if (MouseButtonHeld(MouseButton.Any)) {
                    drawColor = clickColor;
                } else {
                    drawColor = hoverColor;
                }
            }
        }

        public override void OnRender() {
            SetDrawColor(Color4.VA(0, 1));
            DrawRectOutline(2, 0, 0, VW(1), VH(1));

            SetDrawColor(drawColor);
            DrawRect(0, 0, VW(1), VH(1));

            base.OnRender();
        }

    }
}
