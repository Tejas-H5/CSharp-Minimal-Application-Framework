using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class TestError : Element {
        Exception exception;

        public TestError(Exception exception) {
            this.exception = exception;
        }

        public override void OnMount(Window w) {
            SetClearColor(Color4.VA(1, 1));
        }

        public override void OnRender() {
            SetFont("Consolas", 24);

            string text = "An exception occured:\n" +
                exception.Message;

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
            SetDrawColor(Color4.VA(1, 0.5f));
            if (MouseOverSelf) {
                SetDrawColor(Color4.VA(0.5f, 0.5f));
                if (MouseButtonHeld(MouseButton.Any)) {
                    SetDrawColor(Color4.VA(0.5f, 1f));
                }
            }

            Rect(0, 0, Width, Height);

            SetDrawColor(Color4.Black);
            RectOutline(1, 0, 0, Width, Height);
        }

        public override void OnUpdate() {
            if (MouseOverSelf && MouseButtonPressed(MouseButton.Any)) {
                OnClick?.Invoke();
            }
        }
    }

    class UIPair : Element {
        public UIPair(Element el1, Element el2) {
            SetChildren(el1, el2);
        }

        public override void OnRender() {
            SetDrawColor(Color4.Black);

            RectOutline(1, this[0].RelativeRect);
            RectOutline(1, this[1].RelativeRect);
        }

        public override void OnLayout() {
            LayoutTwoSplit(this[0], this[1], Direction.Right, VW(0.5f));
            LayoutChildren();
        }
    }

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
            float prevDepth = CTX.Current2DDepth;
            CTX.Current2DDepth = 0;

            SetFont("Consolas", 12);

            if (GetCharHeight() != textHeight) {
                textHeight = GetCharHeight();
                Layout();
            }

            foreach ((float y, string name, Rect rect) in IterateNames()) {
                SetDrawColor(Color4.White);
                Rect(rect);

                SetDrawColor(0, 0, 0, 1);
                RectOutline(1, rect);

                if (MouseOver(rect)) {
                    SetDrawColor(0, 0, 0, 0.5f);

                    if (MouseButtonHeld(MouseButton.Any)) {
                        SetDrawColor(0.5f, 0.5f, 0.5f, 1);
                    }
                }

                Text(name, VW(0.5f), y, HorizontalAlignment.Center, VerticalAlignment.Top);
            }

            CTX.Current2DDepth = prevDepth;
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

    class ChoiceInput<T> : Element, IInput<T> {
        readonly string[] allNames;
        T[] values;

        T GetValue(string name) {
            return values[Array.IndexOf(allNames, name)];
        }

        string GetName(T obj) {
            return allNames[Array.IndexOf(values, obj)];
        }

        T currentValue;
        string currentName;

        public T Value => currentValue;

        TextInput<string> textInput;
        NameList nameList;

        public event Action<T> OnChanged;
        public event Action<T> OnFinalized;
        bool stayOpen = false;

        public ChoiceInput(string[] names, T[] values, T selected)
            : this(names, values, Array.IndexOf(values, selected)) { }

        public ChoiceInput(string[] names, T[] values, int selected) {
            allNames = names;
            this.values = values;

            string defaultValue = GetValue(names[selected]).ToString();

            textInput = new TextInput<string>(
                new TextElement("", Color4.VA(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Center),
                defaultValue,
                (string s) => s
            );

            textInput.OnChanged += TextInput_OnTextChanged;
            textInput.OnDefocused += TextInput_OnDefocused;

            nameList = new NameList(allNames);
            nameList.OnSelect += NameList_OnSelect;
            nameList.IsVisible = false;

            currentName = allNames[selected];
            currentValue = this.values[selected];

            SetChildren(textInput, nameList);
        }

        private void TextInput_OnDefocused() {
            textInput.String = currentName;
        }

        public static ChoiceInput<object> FromEnum(Type enumType, object defaultValue) {
            if (!enumType.IsEnum) {
                throw new Exception("enumType must be an `enum`.");
            }

            // TODO: can be cached based on type
            var mapping = new Dictionary<string, object>();
            var names = enumType.GetEnumNames();
            var values = new object[names.Length];
            enumType.GetEnumValues().CopyTo(values, 0);

            return new ChoiceInput<object>(names, values, Array.IndexOf(values, defaultValue));
        }


        void NameList_OnSelect(string name) {
            textInput.EndTyping();

            currentName = name;
            textInput.String = name;
            currentValue = GetValue(name);

            OnChanged?.Invoke(currentValue);
            OnFinalized?.Invoke(currentValue);
        }


        public override void OnUpdate() {
            if (!textInput.HasFocus) {
                if (stayOpen) {
                    stayOpen = false;
                } else {
                    return;
                }
            } else {
                stayOpen = true;
            }

            nameList.IsVisible = textInput.HasFocus || stayOpen;
        }


        public override void OnLayout() {
            nameList.RelativeRect = new Rect(0, -nameList.WantedHeight, VW(1), 0);

            LayoutChildren();
        }


        private void TextInput_OnTextChanged(string str) {
            nameList.Filter(str);
        }
    }

    static class TestRunerCommon {
        public static object InstantiateDefaultParameterValue(ParameterInfo parameter) {
            if (parameter.RawDefaultValue != DBNull.Value) {
                if (parameter.ParameterType.IsEnum) {
                    return Enum.ToObject(parameter.ParameterType, parameter.RawDefaultValue);
                } else {
                    return parameter.RawDefaultValue;
                }
            } else {
                return Activator.CreateInstance(parameter.ParameterType);
            }
        }
    }

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


    public class VisualTestRunner : Element {
        List<Type> visualTestElements = new List<Type>();

        TestList testList;
        TextInput<string> searchbox;
        Reflector reflectionPanel;
        MountingContainer mountingContainer;

        public event Action<Element, object[]> OnTestcaseChanged;

        int initialTest = 0;

        public VisualTestRunner(Type initialTestType = null) {
            testList = new TestList(visualTestElements);
            testList.OnSelect += TestList_OnSelect;

            searchbox = new TextInput<string>(
                new TextElement("", Color4.VA(0, 1), "Consolas", 24), "",
                (string s) => s
            );

            searchbox.Placeholder = "Search for a test";
            searchbox.OnChanged += Searchbox_OnTextChanged;

            mountingContainer = new MountingContainer();


            reflectionPanel = new Reflector();

            SetChildren(testList, searchbox, mountingContainer, reflectionPanel);

            MinimalAFEnvironment.Debug = true;

            FindAllVisualTests();

            Console.WriteLine("Found " + visualTestElements.Count + " tests:");
            foreach (Type t in visualTestElements) {
                Console.WriteLine("\t" + t.Name);
            }

            if(initialTestType != null) {
                initialTest = visualTestElements.IndexOf(initialTestType);
            }
        }



        private void Searchbox_OnTextChanged(string value) {
            testList.SetFilter(value);
        }

        private void TestList_OnSelect(Type obj) {
            StartTest(visualTestElements.IndexOf(obj));
        }

        public void StartTest(Type type, object[] args) {
            StartTest(visualTestElements.IndexOf(type), args);
        }

        void StartTest(int index, object[] args = null) {
            try {
                mountingContainer.TestMounting.SetChildren(null);
                int count = visualTestElements.Count;

                currentTest = index;
                Type t = visualTestElements[currentTest];

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
                    if (!Reflector.SupportsType(paramType)) {
                        throw new Exception("The type " + paramType.Name + " on parameter " + expectedArgs[i].Name + " is not yet supported");
                    }

                    args[i] = TestRunerCommon.InstantiateDefaultParameterValue(expectedArgs[i]);
                }

                Element test = (Element)Activator.CreateInstance(t, args);

                mountingContainer.TestMounting.SetChildren(test);
                OnTestcaseChanged?.Invoke(test, args);
            } catch (Exception e) {
                mountingContainer.TestMounting.SetChildren(new TestError(e));
                OnTestcaseChanged?.Invoke(mountingContainer.TestMounting[0], new object[0]);
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
            StartTest(initialTest);
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
