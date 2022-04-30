// Warning: Tight coupling. Not for the faint of heart
using System.Reflection;

namespace MinimalAF {
    class MountingContainer : Element {
        TestMounting mounting;
        VisualTestRunner testRunner;
        string info;

        public TestMounting TestMounting => mounting;

        public MountingContainer() {
            mounting = new TestMounting();
            Clipping = true;

            SetChildren(mounting);
        }

        public override void OnMount(Window w) {
            if(testRunner != null) {
                testRunner.OnTestcaseChanged -= TestRunner_OnTestcaseChanged;
            }

            testRunner = GetAncestor<VisualTestRunner>();

            testRunner.OnTestcaseChanged += TestRunner_OnTestcaseChanged;
        }

        private void TestRunner_OnTestcaseChanged(Element arg1, object[] arg2) {
            var currentTestAttributes = arg1.GetType().GetCustomAttribute<VisualTestAttribute>();

            info = "Test: " + arg1.GetType().Name + "\n\n" +
            "Description: " + currentTestAttributes.Description + "\n\n" +
            "Tags: " + currentTestAttributes.Tags;
        }

        public override void OnRender() {
            SetDrawColor(Color4.Black);
            DrawRectOutline(1, mounting.RelativeRect);

            SetFont("Consolas", 12);
            DrawText(info, 0, 0, HorizontalAlignment.Left, VerticalAlignment.Bottom);
        }

        public override void OnLayout() {
            Rect wanted = mounting.RelativeRect;
            wanted.SetWidth(mounting.WantedSize.Item1, 0.5f);
            wanted.SetHeight(mounting.WantedSize.Item2, 0.5f);
            mounting.RelativeRect = wanted;

            LayoutChildren();
        }
    }
}
