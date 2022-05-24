using System;
using System.Collections.Generic;


namespace MinimalAF {
    class TestList : Element {
        List<(Type, VisualTestAttribute)> visualTestElementsUnfiltered;
        List<(Type, VisualTestAttribute)> visualTestElements = new List<(Type, VisualTestAttribute)>();
        float scrollAmount = 0;
        string filter = "";
        float gap = 3;

        public TestList(List<(Type, VisualTestAttribute)> tests) {
            UpdateTests(tests);
            Clipping = true;
        }

        public void UpdateTests(List<(Type, VisualTestAttribute)> tests) {
            visualTestElementsUnfiltered = tests;
            SetFilter("");
        }

        public event Action<Type> OnSelect;
        float textHeight;

        IEnumerable<(float, (Type, VisualTestAttribute), bool)> IterateTypes() {
            float y = VH(1) - scrollAmount;
            for (int i = 0; i < visualTestElements.Count; i++) {
                y -= textHeight + gap;

                if (y > VH(1)) {
                    continue;
                }

                yield return (y, visualTestElements[i], MouseOver(0, y - gap, VW(1), y + textHeight));
            }
        }

        public override void OnUpdate() {
            scrollAmount += MousewheelNotches * textHeight * 5;

            float maxScroll = -(textHeight + gap) * visualTestElements.Count;
            if (scrollAmount < maxScroll) {
                scrollAmount = maxScroll;
            }

            if (scrollAmount > 0) {
                scrollAmount = 0;
            }

            foreach ((float y, (Type test, VisualTestAttribute testInfo), bool isOver) in IterateTypes()) {
                if (MouseButtonPressed(MouseButton.Left)) {
                    if (isOver) {
                        OnSelect?.Invoke(test);
                    }
                }
            }
        }

        public override void OnRender() {
            SetDrawColor(Color.VA(1, 0.5f));
            DrawRect(0, 0, VW(1), VH(1));

            SetFont("Consolas", 16);
            textHeight = GetCharHeight();

            foreach ((float y, (Type test, VisualTestAttribute testInfo), bool isOver) in IterateTypes()) {
                SetDrawColor(Color.VA(0, 1));
                if (isOver) {
                    SetDrawColor(Color.VA(0.25f, 1));
                    if (MouseButtonHeld(MouseButton.Left)) {
                        SetDrawColor(Color.RGBA(1f, 0, 0, 0.5f));
                    }
                }

                DrawText(test.Name, 10, y);
            }
        }

        internal void SetFilter(string value) {
            filter = value.Trim();

            visualTestElements.Clear();
            foreach (var pair in visualTestElementsUnfiltered) {
                (Type t, VisualTestAttribute testInfo) = pair;
                if (
                    filter != "" &&
                    !(t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    testInfo.Tags.Contains(filter, StringComparison.OrdinalIgnoreCase))
                ) {
                    continue;
                }

                visualTestElements.Add(pair);
            }
        }
    }
}
