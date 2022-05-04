using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MinimalAF {

    class TestParameterUI : Element {
        Element testcase;
        List<(PropertyInfo, IInput<object>)> propertyInputs;
        float charHeight;
        bool skipUpdate = false;

        public override void OnMount(Window w) {
            SetFont("Consolas", 12);
            charHeight = GetCharHeight();
        }

        public override void OnUpdate() {
            if(skipUpdate) {
                skipUpdate = false;
                return;
            }


            if (propertyInputs != null) {
                for (int i = 0; i < propertyInputs.Count; i++) {
                    var (property, input) = propertyInputs[i];

                    if (input.HasFocus)
                        continue;

                    input.Value = property.GetValue(testcase);
                }
            }
        }

        public void SetTestcase(Element element) {
            if (element == testcase)
                return;

            SetChildren(null);
            testcase = element;

            if(element == null) {
                return;
            }

            Type currentTestClass = element.GetType();

            var properties = currentTestClass.GetProperties();
            propertyInputs = new List<(PropertyInfo, IInput<object>)>();
            for (int i = 0; i < properties.Length; i++) {
                var property = properties[i];

                if (!TestRunnerCommon.SupportsType(property.PropertyType))
                    continue;

                if(!property.CanWrite) {
                    continue;
                }

                var input = TestRunnerCommon.CreateInput(property.PropertyType, Activator.CreateInstance(property.PropertyType));
                var label = TestRunnerCommon.CreateText(property.Name);
                var pair = new UIPair(label, (Element)input);

                input.OnChanged += (object obj) => {
                    if(property.CanWrite) {
                        property.SetValue(testcase, obj);
                    }
                    skipUpdate = true;
                };

                AddChild(pair);

                propertyInputs.Add((property, input));
            }
        }

        public override void OnLayout() {
            LayoutLinear(Children, Direction.Down, charHeight + 20, 0, 0);
            LayoutInset(Children, 5);

            LayoutChildren();
        }
    }

    class TestParameterPanel : Element {
        Element currentTest;
        TestParameterUI uiPanel;
        float verticaloffset;

        public TestParameterPanel() {
            SetChildren(
                uiPanel = new TestParameterUI()
            );

            stackingOffset = 1;
        }

        public override void OnMount(Window w) {
            SetFont("Consolas", 16);
            verticaloffset = GetCharHeight() + 5;
        }

        public void SetTestcase(Element element) {
            currentTest = element;
            uiPanel.SetTestcase(element);
        }

        public override void OnRender() {
            SetDrawColor(0, 0, 0, 1);
            DrawRectOutline(1, 0, 0, Width, Height);

            SetDrawColor(1, 1, 1, 0.5f);
            DrawRect(new Rect(0, 0, Width, Height));

            float y = 0, spacing = 5;

            SetFont("Consolas", 16);
            SetDrawColor(0, 0, 0, 1);
            DrawText("Test parameters", VW(0.5f), VH(1), HorizontalAlignment.Center, VerticalAlignment.Top);
            y += GetCharHeight() + spacing;
        }

        public override void OnLayout() {
            LayoutInset(Children, 0, 0, 0, verticaloffset);
            LayoutChildren();
        }

        public override Rect DefaultRect(Rect parentRelativeRect) {
            return new Rect(20, 20, 375, parentRelativeRect.Height - 20);
        }
    }
}
