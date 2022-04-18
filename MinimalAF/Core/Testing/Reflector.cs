using System;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class Reflector : Element {
        VisualTestRunner testRuner;
        Button restartButton;
        Type currentTestClass;
        float scrollOffset = 0;
        object[] args;

        public object[] Args => args;

        public Reflector() {
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

        TextElement CreateText(string name) {
            return new TextElement(name, Color4.Black, "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Center);
        }

        public static bool SupportsType(Type type) {
            return type == typeof(int)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(long)
                || type == typeof(string)
                || type == typeof(bool)
                || type.IsEnum;
        }

        IInput<object> CreateInput(Type type, object defaultValue) {
            IInput<object> input = null;

            // TODO: add a dropdown that lets you select from an enum for enums
            if (type == typeof(int)) {
                input = new TextInput<object>(CreateText(""), defaultValue, (string s) => int.Parse(s));
            } else if (type == typeof(float)) {
                input = new TextInput<object>(CreateText(""), defaultValue, (string s) => float.Parse(s));
            } else if (type == typeof(double)) {
                input = new TextInput<object>(CreateText(""), defaultValue, (string s) => double.Parse(s));
            } else if (type == typeof(long)) {
                input = new TextInput<object>(CreateText(""), defaultValue, (string s) => long.Parse(s));
            } else if (type == typeof(string)) {
                input = new TextInput<object>(CreateText(""), defaultValue, (string s) => s);
            } else if (type == typeof(bool)) {
                input = new ChoiceInput<object>(new string[] { "True", "False" }, new object[] { true, false }, defaultValue);
            } else if (type.IsEnum) {
                input = ChoiceInput<object>.FromEnum(type, defaultValue);
            }

            return input;
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

                args[i] = TestRunerCommon.InstantiateDefaultParameterValue(parameter);

                var input = CreateInput(parameter.ParameterType, args[i]);
                var label = CreateText(parameter.Name);
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
