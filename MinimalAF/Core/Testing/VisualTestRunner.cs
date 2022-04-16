using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class NoMoreTests : Element {
        public override void OnMount(Window w) {
            SetClearColor(Color4.VA(1, 1));
        }

        public override void OnRender() {
            SetFont("Consolas", 24);

            string text = "No tests.\nGive the [" + typeof(VisualTestAttribute).Name + "] attribute to an element"
                + "\nin order to make it show up here. ";

            Text(text, VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
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
            Clipping = true;
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
            scrollAmount += MousewheelNotches * textHeight * 5;

            float maxScroll = -(textHeight + gap) * visualTestElements.Count;
            if (scrollAmount < maxScroll) {
                scrollAmount = maxScroll;
            }

            if (scrollAmount > 0) {
                scrollAmount = 0;
            }

            foreach ((float y, Type test, bool isOver) in IterateTypes()) {
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

        internal void SetFilter(string value) {
            filter = value.Trim();

            visualTestElements.Clear();
            foreach (Type t in visualTestElementsUnfiltered) {
                if (filter != "" && !t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                visualTestElements.Add(t);
            }
        }
    }


    class TestMounting : Window {
        ApplicationWindow window;
        public (float, float) WantedSize;

        public override void OnRender() {
            RenderDragHandle(Direction.Left);
            RenderDragHandle(Direction.Up);
            RenderDragHandle(Direction.Right);
            RenderDragHandle(Direction.Down);

            SetDrawColor(Color4.Black);

            string infoText = "size: " + WantedSize.Item1.ToString("0.00") + ", " + WantedSize.Item2.ToString("0.00")
                + "\n(WindowState." + WindowState.ToString()
                + ") \nUpdateFPS:" + UpdateFrequency.ToString("0.0") + "\nRenderFPS:"
                + RenderFrequency.ToString("0.0");

            SetFont("Consolas", 12);
            Text(infoText, 0, 0, HorizontalAlignment.Left, VerticalAlignment.Top);
        }


        public override void OnUpdate() {
            UpdateDragHandle(Direction.Left);
            UpdateDragHandle(Direction.Up);
            UpdateDragHandle(Direction.Right);
            UpdateDragHandle(Direction.Down);

            if (isDragging) {
                float newWantedX = 2 * dragX * MouseDragDeltaX + startWidth;
                float newWantedY = 2 * dragY * MouseDragDeltaY + startHeight;

                WantedSize = (newWantedX, newWantedY);

                TriggerLayoutRecalculation();
            }
        }

        const float EDGE_WIDTH = 40;

        (Rect, float, float) DragParameters(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    return (new Rect(-EDGE_WIDTH, Height, Width + EDGE_WIDTH, Height + EDGE_WIDTH), 0, 1);
                case Direction.Down:
                    return (new Rect(-EDGE_WIDTH, -EDGE_WIDTH, Width + EDGE_WIDTH, 0), 0, -1);
                case Direction.Left:
                    return (new Rect(-EDGE_WIDTH, -EDGE_WIDTH, 0, Height + EDGE_WIDTH), -1, 0);
                case Direction.Right:
                    return (new Rect(Width, -EDGE_WIDTH, Width + EDGE_WIDTH, Height + EDGE_WIDTH), 1, 0);
                default:
                    throw new Exception("Invalid direction " + direction.ToString());
            }
        }

        void RenderDragHandle(Direction dir) {
            (Rect hitbox, float x, float y) = DragParameters(dir);

            if (MouseOver(hitbox)) {
                SetDrawColor(Color4.RGBA(0, 0, 1, 0.5f));
                Rect(hitbox);
            }
        }

        bool isDragging = false;
        float dragX = 0, dragY = 0;
        float startWidth, startHeight;

        void UpdateDragHandle(Direction dir) {
            (Rect hitbox, float x, float y) = DragParameters(dir);

            if (Input.Mouse.StartedDragging && MouseOver(hitbox)) {
                isDragging = true;
                if (dir == Direction.Left || dir == Direction.Right) {
                    dragX = x;
                } else {
                    dragY = y;
                }

                startWidth = WantedSize.Item1;
                startHeight = WantedSize.Item2;
            }

            if (MouseFinishedDragging) {
                isDragging = false;
                dragX = 0;
                dragY = 0;
            }
        }

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
                WantedSize = value;
                Layout();
            }
        }

        public override void OnMount(Window w) {
            window = Parent.GetAncestor<ApplicationWindow>();
            window.Title = "Visual Test Runner";
            window.Size = (900, 600);
            window.WindowState = WindowState.Maximized;
        }
    }

    class Button : Element {
        public event Action OnClick;

        public Button(string text) {
            SetChildren(new TextElement(text, Color4.VA(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Center));
        }

        public override void OnRender() {
            //SetDrawColor(Color4.VA(1, 0.5f));
            //if(MouseOverSelf) {
            //    SetDrawColor(Color4.VA(0.5f, 0.5f));
            //    if(MouseButtonHeld(MouseButton.Any)) {
            //        SetDrawColor(Color4.VA(0.5f, 1f));
            //    }
            //}

            SetDrawColor(Color4.Red);

            Rect(0, 0, Width, Height);
        }

        public override void OnUpdate() {
            if (MouseOverSelf && MouseButtonPressed(MouseButton.Any)) {
                OnClick?.Invoke();
            }
        }
    }

    class Reflector : Element {
        VisualTestRunner testRuner;
        Button restartButton;
        Type currentTestClass;
        float scrollOffset = 0;

        public Reflector() {
            restartButton = new Button("Restart");
            restartButton.OnClick += RestartButton_OnClick;
        }

        private void RestartButton_OnClick() {
            testRuner.StartTest(currentTestClass);
        }

        public override void OnMount(Window w) {
            testRuner = GetAncestor<VisualTestRunner>();
            testRuner.OnTestcaseChanged += TestRuner_OnTestcaseChanged; //


        }

        private void TestRuner_OnTestcaseChanged(Element obj) {
            if (obj.GetType() == currentTestClass) {
                return;
            }
            currentTestClass = obj.GetType();

            SetChildren(null);

            var constructor = currentTestClass.GetConstructors()[0];
            
            // TODO: add editable fields for all the constructor parameters
            var parameters = constructor.GetParameters();
            foreach(var parameter in parameters) {

            }

            AddChild(restartButton);
        }

        public override void OnLayout() {
            LayoutLinear(Children, Direction.Down, 40, scrollOffset, 5);
            LayoutInset(Children, 5);

            LayoutChildren();
        }
    }

    class MountingContainer : Element {
        TestMounting mounting;

        public TestMounting TestMounting => mounting;

        public MountingContainer() {
            mounting = new TestMounting();
            Clipping = true;

            SetChildren(mounting);
        }

        public override void OnRender() {
            SetDrawColor(Color4.Black);
            RectOutline(1, mounting.RelativeRect);
        }

        public override void OnLayout() {
            Rect wanted = mounting.RelativeRect;
            wanted.SetWidth(mounting.WantedSize.Item1, 0.5f);
            wanted.SetHeight(mounting.WantedSize.Item2, 0.5f);
            mounting.RelativeRect = wanted;

            LayoutChildren();
        }
    }


    // [VisualTest] This actually works btw.
    public class VisualTestRunner : Element {
        List<Type> visualTestElements = new List<Type>();

        TestList testList;
        TextInput<string> searchbox;
        Reflector reflectionPanel;
        MountingContainer mountingContainer;

        public event Action<Element> OnTestcaseChanged;

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

            mountingContainer = new MountingContainer();


            reflectionPanel = new Reflector();

            SetChildren(testList, searchbox, mountingContainer, reflectionPanel);
        }



        private void Searchbox_OnTextChanged(string value) {
            testList.SetFilter(value);
        }

        private void TestList_OnSelect(Type obj) {
            StartTest(visualTestElements.IndexOf(obj));
        }

        public void StartTest(Type type) {
            StartTest(visualTestElements.IndexOf(type));
        }

        void StartTest(int index) {
            mountingContainer.TestMounting.SetChildren(null);

            // a funny recursive case I added. this is so we won't get a stack overflow with 0
            // other test cases. TODO: remove this 
            int count = visualTestElements.Count;
            if (count == 1 && visualTestElements[0].GetType() == typeof(VisualTestRunner)) {
                count = 0;
            }

            if (index < 0 || index >= visualTestElements.Count) {
                mountingContainer.TestMounting.SetChildren(new NoMoreTests());
                OnTestcaseChanged?.Invoke(mountingContainer.TestMounting[0]);
                return;
            }

            currentTest = index;
            Type t = visualTestElements[currentTest];

            var constructors = t.GetConstructors();
            if (constructors.Length == 0) {
                throw new Exception("The class " + t.Name + " needs to have at least one constructor");
            } else if (constructors.Length > 1) {
                throw new Exception("The class " + t.Name + " cannot have more than one constructor to be testable here.");
            }

            try {
                var constructor = constructors[0];
                var expectedArgs = constructor.GetParameters();
                object[] args = new object[expectedArgs.Length];

                for (int i = 0; i < expectedArgs.Length; i++) {
                    var exArg = expectedArgs[i];

                    if (exArg.RawDefaultValue != null) {
                        args[i] = exArg.RawDefaultValue;
                        continue;
                    }

                    object defaultVal = Activator.CreateInstance(exArg.ParameterType);
                    args[i] = defaultVal;
                }

                Element test = (Element)Activator.CreateInstance(t, args);

                mountingContainer.TestMounting.SetChildren(test);
                OnTestcaseChanged?.Invoke(test);
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



        public override void AfterMount(Window w) {
            MinimalAFEnvironment.Debug = true;

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

            RectOutline(1, testList.RelativeRect);
            RectOutline(1, searchbox.RelativeRect);
            RectOutline(1, reflectionPanel.RelativeRect);
            RectOutline(1, mountingContainer.RelativeRect);

            SetDrawColor(Color4.VA(1, .5f));
            Rect(searchbox.RelativeRect);
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
