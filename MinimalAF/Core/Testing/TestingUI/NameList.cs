using System;
using System.Collections.Generic;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class NameList : Element {
        readonly string[] allNames;
        List<string> filterednames = new List<string>();
        float textHeight;
        float wantedHeight;
        public float WantedHeight => wantedHeight;

        public Action<string> OnSelect;

        public NameList(string[] names) {
            allNames = names;

            Filter("");
            StackingOffset = 1;

            Debug = true;
        }

        public bool IsValid(string value) {
            return Array.IndexOf(allNames, value) != -1;
        }

        IEnumerable<(float, string, Rect)> IterateNames() {
            float y = 0;
            float gap = 5;
            for (int i = 0; i < filterednames.Count; i++) {
                yield return (y, filterednames[i], new Rect(0, y - textHeight - gap, VW(1), y));

                y -= textHeight + gap;
            }

            wantedHeight = -y;
        }

        public override void OnUpdate() {
            foreach ((float y, string name, Rect rect) in IterateNames()) {
                if (MouseOver(rect) && Input.Mouse.ButtonPressed(MouseButton.Any)) {
                    OnSelect?.Invoke(name);
                    break;
                }
            }
        }

        public override void OnRender() {
            SetFont("Consolas", 16);

            if (GetCharHeight() != textHeight) {
                textHeight = GetCharHeight();
                Layout();
            }

            foreach ((float y, string name, Rect rect) in IterateNames()) {
                SetDrawColor(Color4.White);
                DrawRect(rect);

                SetDrawColor(0, 0, 0, 1);
                DrawRectOutline(1, rect);

                if (MouseOver(rect)) {
                    SetDrawColor(0, 0, 0, 0.5f);

                    if (MouseButtonHeld(MouseButton.Any)) {
                        SetDrawColor(0.5f, 0.5f, 0.5f, 1);
                    }
                }

                DrawText(name, VW(0.5f), y, HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }

        public void Filter(string str) {
            filterednames.Clear();

            for (int i = 0; i < allNames.Length && i < 5; i++) {
                if (!allNames[i].Contains(str, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                filterednames.Add(allNames[i]);
            }
        }
    }
}
