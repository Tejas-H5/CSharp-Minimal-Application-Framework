using System;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class ConstructorParameterPanel : Element {
        VisualTestRunner testRuner;
        Button restartButton;
        Type currentTestClass;
        float scrollOffset = 0;
        object[] args;

        public object[] Args => args;

        public ConstructorParameterPanel() {
            restartButton = new Button("Restart");
            restartButton.OnClick += RestartButton_OnClick;
        }

        private void RestartButton_OnClick() {
            testRuner.StartTest(currentTestClass, args);
        }

        public override void OnMount(Window w) {
            testRuner = GetAncestor<VisualTestRunner>();
            testRuner.OnTestcaseChanged += TestRuner_OnTestcaseChanged;
        }


        private void TestRuner_OnTestcaseChanged(Element obj, object[] args) {
            if (obj.GetType() == currentTestClass) {
                return;
            }
            currentTestClass = obj.GetType();

            SetChildren(null);

            var constructor = currentTestClass.GetConstructors()[0];
            this.args = args;

            var parameters = constructor.GetParameters();
            for (int i = 0; i < args.Length; i++) {
                var parameter = parameters[i];

                args[i] = TestRunnerCommon.InstantiateDefaultParameterValue(parameter);

                var input = TestRunnerCommon.CreateInput(parameter.ParameterType, args[i]);
                var label = TestRunnerCommon.CreateText(parameter.Name);
                var pair = new UIPair(label, (Element)input);
                // not a redundant assigment, requried by the following closure
                int argumentIndex = i;
                input.OnChanged += (object obj) => {
                    args[argumentIndex] = obj;
                };

                AddChild(pair);
            }

            AddChild(restartButton);
        }


        public override void OnLayout() {
            LayoutLinear(Children, Direction.Down, 40, scrollOffset, 5);
            LayoutInset(Children, 5);

            LayoutChildren();
        }
    }
}
