using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MinimalAF {
    class NoMoreTests : Element {
        public override void OnMount(Window w) {
            SetClearColor(Color4.VA(1, 1));
        }

        public override void OnRender() {
            SetFont("Consolas", 24);

            Text("No more tests. Press esc to close", VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Escape)) {
                GetAncestor<ApplicationWindow>().Close();
            }
        }
    }

    class TestList : Element {
        List<Type> visualTestElementsUnfiltered;
        List<Type> visualTestElements = new List<Type>();
        float scrollAmount = 0;
        string filter = "";
        float gap = 3;

        public TestList(List<Type> tests) {
            UpdateTests(tests);
        }

        public void UpdateTests(List<Type> tests) {
            visualTestElementsUnfiltered = tests;
            SetFilter("");
        }

        public event Action<Type> OnSelect;
        float textHeight;

        IEnumerable<(float, Type, bool)> IterateTypes() {
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
            scrollAmount += MousewheelNotches * textHeight;

            float maxScroll = -(textHeight + gap) * visualTestElements.Count;
            if(scrollAmount < maxScroll) {
                scrollAmount = maxScroll;
            }

            if(scrollAmount > 0) {
                scrollAmount = 0;
            }

            foreach((float y, Type test, bool isOver) in IterateTypes()) {
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
            SetClippingRect(_screenRect);

            SetFont("Consolas", 12);
            textHeight = GetCharHeight();

            foreach ((float y, Type test, bool isOver) in IterateTypes()) {
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

        public override void AfterRender() {
            ClearClippingRect();
        }

        internal void SetFilter(string value) {
            filter = value.Trim();

            visualTestElements.Clear();
            foreach(Type t in visualTestElementsUnfiltered) {
                if (filter != "" && !t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                visualTestElements.Add(t);
            }
        }
    }

    public class VisualTestRunner : Window {
        List<Type> visualTestElements = new List<Type>();

        TestList testList;
        TextInput<string> searchbox;
        ApplicationWindow window;
        Element currentTestElement;
        (int, int) wantedSize;

        public VisualTestRunner() {
            testList = new TestList(visualTestElements);
            testList.OnSelect += TestList_OnSelect;

            searchbox = new TextInput<string>(
                new TextElement("", Color4.VA(0, 1), "Consolas", 24),
                new Property<string>(""),
                (string s) => s
            );

            searchbox.Placeholder = "Search for a test";
            searchbox.OnTextChanged += Searchbox_OnTextChanged;

            SetChildren(testList, searchbox);
        }

        private void Searchbox_OnTextChanged(string value) {
            testList.SetFilter(value);
        }

        private void TestList_OnSelect(Type obj) {
            StartTest(visualTestElements.IndexOf(obj));
        }

        void StartTest(int index) {
            RemoveChild(currentTestElement);
            currentTestElement = null;

            if (index < 0 || index >= visualTestElements.Count) {
                ranOutOfTests = true;
                currentTestElement = new NoMoreTests();
                AddChild(currentTestElement);
                return;
            }

            currentTest = index;
            Type t = visualTestElements[currentTest];

            try {
                Element test = (Element)Activator.CreateInstance(t);
                currentTestElement = test;

                AddChild(test);
            } catch (MissingMethodException) {
                throw new Exception("The class " + t.Name + " needs to have a parameterless constructor" +
                    "( e.g. var x = new " + t.Name + "() ) in order to be instantiated by this test harness.");
            } catch (Exception e) {
                throw e;
            }
        }

        public void FindAllVisualTests() {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in assembly.GetTypes()) {
                    if (t.GetCustomAttribute(typeof(VisualTestAttribute)) == null) {
                        continue;
                    }

                    if (t.BaseType != typeof(Element)) {
                        throw new Exception("Class " + t.Name + " does not inherit from the " + typeof(Element).Name + " class. You may have put the [VisualTestAttribute] on it by accident");
                    }

                    visualTestElements.Add(t);
                }
            }

            testList.UpdateTests(visualTestElements);
        }



        public override void OnMount(Window w) {
            MinimalAFEnvironment.Debug = true;

            window = Parent.GetAncestor<ApplicationWindow>();
            window.Title = "Visual Test Runner";
            window.Size = (900, 600);
            window.WindowState = WindowState.Maximized;

            FindAllVisualTests();

            Console.WriteLine("Found " + visualTestElements.Count + " tests:");
            foreach (Type t in visualTestElements) {
                Console.WriteLine("\t" + t.Name);
            }

            Console.WriteLine("Press escape to go to the next test");

            StartTest(0);
        }

        public override void OnRender() {
            SetDrawColor(Color4.VA(0, 1));

            RectOutline(1, rightSectionRect);
            RectOutline(1, testRect);
            RectOutline(1, testList.RelativeRect);
            RectOutline(1, searchbox.RelativeRect);

            SetDrawColor(Color4.VA(1, .5f));
            Rect(searchbox.RelativeRect);

            if (!ranOutOfTests)
                return;
        }

        int currentTest = 0;
        bool ranOutOfTests = false;

        public override string Title {
            get => window.Title;
            set => window.Title = value;
        }

        public override double UpdateFrequency {
            get => window.UpdateFrequency;
            set => window.UpdateFrequency = value;
        }

        public override double RenderFrequency {
            get => window.RenderFrequency;
            set => window.RenderFrequency = value;
        }

        public override WindowState WindowState {
            get;
            set;
        }

        public override (int, int) Size {
            get => ((int)RelativeRect.Width, (int)RelativeRect.Height);
            set {
                wantedSize = value;
                Layout();
            }
        }

        Rect rightSectionRect, testRect;

        public override void OnLayout() {
            float sidePanelWidth = VW(0.25f);
            float inset = 5;
            rightSectionRect = new Rect(sidePanelWidth, 0, VW(1), VH(1))
                .Inset(inset);

            testList.RelativeRect = new Rect(0, 0, sidePanelWidth, VH(1) - 50)
                .Inset(inset);

            searchbox.RelativeRect = new Rect(0, VH(1) - 50, sidePanelWidth, VH(1))
                .Inset(inset);

            testRect = rightSectionRect;
            testRect.SetWidth(wantedSize.Item1, 0.5f);
            testRect.SetHeight(wantedSize.Item2, 0.5f);
            currentTestElement.RelativeRect = testRect;

            LayoutChildren();
        }
    }
}
