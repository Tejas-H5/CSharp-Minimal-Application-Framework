﻿using System;
using System.Collections.Generic;

// Warning: Tight coupling. Not for the faint of heart
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
            SetDrawColor(Color4.VA(1, 0.5f));
            Rect(0, 0, VW(1), VH(1));

            SetFont("Consolas", 12);
            textHeight = GetCharHeight();

            foreach ((float y, (Type test, VisualTestAttribute testInfo), bool isOver) in IterateTypes()) {
                SetDrawColor(Color4.VA(0, 1));
                if (isOver) {
                    SetDrawColor(Color4.VA(0.25f, 1));
                    if (MouseButtonHeld(MouseButton.Left)) {
                        SetDrawColor(Color4.RGBA(1f, 0, 0, 0.5f));
                    }
                }

                Text(test.Name, 10, y);
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