
using System.Reflection;

namespace MinimalAF {
    class MountingContainer : Element {
        VisualTestRunner testRunner;
        string info;

        TestMounting mounting;
        TestParameterPanel testParameterPanel;

        public MountingContainer() {
            mounting = new TestMounting();
            testParameterPanel = new TestParameterPanel();

            SetChildren(mounting, testParameterPanel);
        }

        public void SetTestcase(Element element) {
            mounting.SetTestcase(element);

            testParameterPanel.SetTestcase(element);
        }

        public override void OnMount(Window w) {
            if (testRunner != null) {
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
            SetFont("Consolas", 16);
            SetDrawColor(0, 0, 0, 1);
            DrawText(info, 0, 0, HAlign.Left, VAlign.Bottom);
        }
    }
}
