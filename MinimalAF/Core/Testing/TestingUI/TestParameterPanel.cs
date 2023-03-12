//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace MinimalAF {

//    class TestParameterUI : Element {
//        Element testcase;
//        List<(PropertyInfo, IInput<object>)> propertyInputs;
//        float charHeight;
//        bool skipUpdate = false;

//        public override void OnMount() {
//            SetFont("Consolas", 16);
//            charHeight = GetCharHeight();
//        }


//        public override void OnUpdate() {
//            if (skipUpdate) {
//                skipUpdate = false;
//                return;
//            }


//            if (propertyInputs != null) {
//                for (int i = 0; i < propertyInputs.Count; i++) {
//                    var (property, input) = propertyInputs[i];

//                    if (input.HasFocus)
//                        continue;

//                    input.Value = property.GetValue(testcase);
//                }
//            }
//        }

//        void Render(FrameworkContext ctx) {
//            var y = ctx.Rect.Y1;

//            ctx.Rect = ctx.Rect.Inset(10);
//            foreach(var (prop, input) in propertyInputs) {
//                //y += input.Render(ctx);
//            }
//        }

//        void Update() {
//            foreach (var (prop, input) in propertyInputs) {
//                //input.Update();
//            }
//        }

//        public void SetTestcase(Element element) {
//            if (element == testcase)
//                return;

//            testcase = element;

//            if (element == null) {
//                return;
//            }

//            Type currentTestClass = element.GetType();

//            var properties = currentTestClass.GetProperties();
//            propertyInputs = new List<(PropertyInfo, IInput<object>)>();
//            for (int i = 0; i < properties.Length; i++) {
//                var property = properties[i];

//                if (property.GetCustomAttribute<IgnorePropertyTestingAttribute>() != null) {
//                    continue;
//                }

//                if (!TestRunnerCommon.SupportsType(property.PropertyType))
//                    continue;

//                if (!property.CanWrite) {
//                    continue;
//                }

//                var input = TestRunnerCommon.CreateInput(property.PropertyType, Activator.CreateInstance(property.PropertyType));
//                var label = TestRunnerCommon.CreateText(property.Name);
//                var pair = new UIPair(label, (Element)input);

//                input.OnChanged += (object obj) => {
//                    if (property.CanWrite) {
//                        property.SetValue(testcase, obj);
//                    }
//                    skipUpdate = true;
//                };

//                propertyInputs.Add((property, input));
//            }
//        }

//        //public override void OnLayout() {
//        //    LayoutInset(Children, 5);

//        //    float height = LayoutLinear(Children, Direction.Down, -1, 0, 10);
//        //    RelativeRect = RelativeRect.ResizedHeight(height, 1);
//        //}
//    }

//    class TestParameterPanel : Element {
//        Element _currentTest;
//        readonly TestParameterUI _uiPanel;

//        float _titleHeight, _startX, _startY;
//        bool _isDragging = false;

//        public TestParameterPanel() {
//            //SetChildren(
//            //    _uiPanel = new TestParameterUI()
//            //);

//            StackingOffset = 10000;

//            _uiPanel.Pivot = Vec2(0, 1);
//            Pivot = Vec2(0, 1);
//        }

//        public override void OnMount() {
//            SetFont("Consolas", 16);
//            _titleHeight = GetCharHeight() + 5;

//            Offset = Vec2(0, 0);
//        }

//        public void SetTestcase(Element element) {
//            _currentTest = element;
//            _uiPanel.SetTestcase(element);
//        }

//        public override void OnUpdate() {
//            if (MouseOver(0, ctx.Height * 1 - _titleHeight, ctx.Width * 1, ctx.Height * 1) && MouseStartedDragging) {
//                _isDragging = true;
//                _startX = Offset.X;
//                _startY = Offset.Y;
//            } else if (MouseStoppedDraggingAnywhere) {
//                _isDragging = false;
//            }

//            if (_isDragging) {
//                Offset = Vec2(_startX + MouseDragDeltaX, _startY + MouseDragDeltaY);
//                //ConstrainOffsetToParent();
//            }
//        }

//        public override void OnRender() {
//            ctx.SetDrawColor(0, 0, 0, 1);
//            DrawRectOutline(1, 0, 0, Width, Height);

//            ctx.SetDrawColor(1, 1, 1, 0.5f);
//            DrawRect(new Rect(0, 0, Width, Height));

//            SetFont("Consolas", 16);
//            ctx.SetDrawColor(0, 0, 0, 1);
//            DrawText("Test parameters", ctx.Width * 0.5f, ctx.Height * 1, HAlign.Center, VAlign.Top);
//        }


//        //public override void OnLayout() {
//        //    ConstrainOffsetToParent();
//        //    LayoutInset(Children, 0, 0, 0, _titleHeight);
//        //    LayoutChildren();
//        //}

//        //public override Rect DefaultRect(float pw, float ph) {
//        //    var rect = Rect.PivotSize(
//        //        300, _titleHeight + Children[0].Height, Pivot.X, Pivot.Y
//        //    );

//        //    return rect;
//        //}
//    }
//}
