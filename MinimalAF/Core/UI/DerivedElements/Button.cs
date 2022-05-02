using System;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class Button : Element {
        public event Action OnClick;

        public Button(string text) {
            SetChildren(new TextElement(text, Color4.VA(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Center));
        }

        public override void OnRender() {
            SetDrawColor(Color4.VA(1, 0.5f));
            if (MouseOverSelf) {
                SetDrawColor(Color4.VA(0.5f, 0.5f));
                if (MouseButtonHeld(MouseButton.Any)) {
                    SetDrawColor(Color4.VA(0.5f, 1f));
                }
            }

            DrawRect(0, 0, Width, Height);

            SetDrawColor(Color4.Black);
            DrawRectOutline(1, 0, 0, Width, Height);
        }

        public override void OnUpdate() {
            if (MouseOverSelf && MouseButtonPressed(MouseButton.Any)) {
                OnClick?.Invoke();
            }
        }
    }
}
