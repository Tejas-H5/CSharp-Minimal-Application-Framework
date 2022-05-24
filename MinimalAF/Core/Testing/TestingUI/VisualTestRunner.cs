using System;
using System.Collections.Generic;
using System.Reflection;


namespace MinimalAF {
    public class VisualTestRunner : Element {
        List<(Type, VisualTestAttribute)> visualTestElements = new List<(Type, VisualTestAttribute)>();
        TestList testList;
        TextInput<string> searchbox;
        ConstructorParameterPanel reflectionPanel;
        MountingContainer mountingContainer;

        public event Action<Element, object[]> OnTestcaseChanged;

        int initialTest = 0;

        int IndexOf(Type test) {
            for (int i = 0; i < visualTestElements.Count; i++) {
                if (visualTestElements[i].Item1 == test) {
                    return i;
                }
            }

            return -1;
        }


        public VisualTestRunner(Type initialTestType = null) {
            testList = new TestList(visualTestElements);
            testList.OnSelect += TestList_OnSelect;

            searchbox = new TextInput<string>(
                new TextElement("", Color.VA(0, 1), "Consolas", 24), "",
                (string s) => s
            );

            searchbox.Placeholder = "Search for a test";
            searchbox.OnChanged += Searchbox_OnTextChanged;

            mountingContainer = new MountingContainer();
            reflectionPanel = new ConstructorParameterPanel();



            SetChildren(testList, searchbox, mountingContainer, reflectionPanel);



            FindAllVisualTests();

            Console.WriteLine("Found " + visualTestElements.Count + " tests:");
            foreach ((Type t, VisualTestAttribute info) in visualTestElements) {
                Console.WriteLine("\t" + t.Name);
            }

            if (initialTestType != null) {
                initialTest = IndexOf(initialTestType);
            }
        }

        private void Searchbox_OnTextChanged(string value) {
            testList.SetFilter(value);
        }

        private void TestList_OnSelect(Type obj) {
            StartTest(IndexOf(obj));
        }

        public void StartTest(Type type, object[] args) {
            StartTest(IndexOf(type), args);
        }

        void StartTest(int index, object[] args = null) {
            try {
                mountingContainer.SetTestcase(null);
                int count = visualTestElements.Count;

                currentTest = index;
                (Type t, VisualTestAttribute info) = visualTestElements[currentTest];

                var constructors = t.GetConstructors();
                if (constructors.Length == 0) {
                    throw new Exception("The class " + t.Name + " needs to have at least one constructor");
                } else if (constructors.Length > 1) {
                    throw new Exception("The class " + t.Name + " cannot have more than one constructor to be testable here.");
                }

                var constructor = constructors[0];
                var expectedArgs = constructor.GetParameters();

                if (args == null) {
                    args = new object[expectedArgs.Length];
                }

                for (int i = 0; i < expectedArgs.Length; i++) {
                    if (args[i] != null) {
                        continue;
                    }

                    Type paramType = expectedArgs[i].ParameterType;


                    args[i] = TestRunnerCommon.InstantiateDefaultParameterValue(expectedArgs[i]);
                }

                Element test = (Element)Activator.CreateInstance(t, args);

                mountingContainer.SetTestcase(test);
                OnTestcaseChanged?.Invoke(test, args);
            } catch (Exception e) {
                var nullTest = new TestError(e);
                mountingContainer.SetTestcase(nullTest);
                OnTestcaseChanged?.Invoke(nullTest, new object[0]);
            }
        }

        public void FindAllVisualTests() {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in assembly.GetTypes()) {
                    VisualTestAttribute testInfo = t.GetCustomAttribute<VisualTestAttribute>();
                    if (testInfo == null) {
                        continue;
                    }

                    if (t.BaseType != typeof(Element)) {
                        throw new Exception("Class " + t.Name + " does not inherit from the " + typeof(Element).Name + " class. You may have put the [VisualTestAttribute] on it by accident");
                    }

                    visualTestElements.Add((t, testInfo));
                }
            }

            testList.UpdateTests(visualTestElements);
        }



        public override void AfterMount(Window w) {
            StartTest(initialTest);
        }

        public override void OnRender() {
            SetDrawColor(Color.VA(0, 1));

            DrawRectOutline(1, testList.RelativeRect);
            DrawRectOutline(1, searchbox.RelativeRect);
            DrawRectOutline(1, reflectionPanel.RelativeRect);
            DrawRectOutline(1, mountingContainer.RelativeRect);

            SetDrawColor(Color.VA(1, .5f));
            DrawRect(searchbox.RelativeRect);
        }

        int currentTest = 0;

        public override void OnLayout() {
            float sidePanelWidth = VW(0.25f);
            float inset = 5;
            mountingContainer.RelativeRect = new Rect(sidePanelWidth, 0, VW(1), VH(1))
                .Inset(inset);

            testList.RelativeRect = new Rect(0, VH(0.5f), sidePanelWidth, VH(1) - 50)
                .Inset(inset);

            searchbox.RelativeRect = new Rect(0, VH(1) - 50, sidePanelWidth, VH(1))
                .Inset(inset);

            reflectionPanel.RelativeRect = new Rect(0, 0, sidePanelWidth, VH(0.5f))
                .Inset(inset);

            LayoutChildren();
        }
    }
}
