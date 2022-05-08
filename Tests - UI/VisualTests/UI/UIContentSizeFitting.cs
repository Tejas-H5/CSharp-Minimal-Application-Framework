using System;

namespace MinimalAF.VisualTests.UI {


    public class SizedSquare : Element {
        Color4 _col;
        float _width, _height;
        string _message;

        public SizedSquare(Random rand, float width, float height, string message) {
            _col = Color4.HSV((float)rand.NextDouble() * 360, 0.5f, 1f);
            _width = width;
            _height = height;
            _message = message;
        }

        public override void OnRender() {
            SetDrawColor(_col);
            DrawRect(0, 0, Width, Height);

            SetDrawColor(0, 0, 0, 1);
            DrawRectOutline(1, 0, 0, Width, Height);

            SetFont("Arial", 15);
            SetDrawColor(0, 0, 0, 1);
            DrawText(_message, VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
        }


        public override void OnLayout() {
            LayoutChildren();
        }

        public override Rect DefaultRect(float parentWidth, float parentHeight) {
            return new Rect(0, 0, _width, _height);
        }
    }

    class UIPackageThinggy : Element {
        readonly Random rand = new Random();

        public UIPackageThinggy() {
            RefreshChildren();
        }

        public void RefreshChildren() {
            SetChildren(null);
            for (int i = 0; i < 10; i++) {
                float width = rand.Next(50, 200);
                float height = rand.Next(50, 200);
                var sq = new SizedSquare(rand, width, height, "" + width + ", " + height);
                // sq.Debug = true;
                AddChild(sq);
            }
        }


        public override void OnLayout() {
            LayoutX0(Children, 10);
            LayoutX1(Children, VW(1) - 10);
            float height = LayoutLinear(Children, Direction.Down); 
            RelativeRect = RelativeRect
                .ResizedHeight(height, 1);

            LayoutChildren();
        }
    }


    [VisualTest(
        description: @"Test laying out UI in a row based on their own wanted sizes",
        tags: "UI, layout"
    )]
    public class UIContentSizeFittingTest : Element {
        float _scrollOffset = 0;

        public UIContentSizeFittingTest() {
            for(int i = 0; i < 10; i++) {
                var package = new UIPackageThinggy();
                package.Debug = true;
                AddChild(package);
            }
        }


        public override void OnUpdate() {
            SetFont("Consolas", 16);
            SetDrawColor(0, 0, 0, 0.5f);
            DrawText("Press space to refresh the children", 0, 0);

            if(KeyPressed(KeyCode.Space)) {
                foreach(var(i, e) in Children) {
                    ((UIPackageThinggy)e).RefreshChildren();
                }
            }


            if(MathF.Abs(MousewheelNotches) > 0.01f) {
                _scrollOffset += MousewheelNotches * 50;
                TriggerLayoutRecalculation();
            }
        }

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "UIAspectRatioTest";
            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        public override void OnLayout() {
            LayoutX0(Children, 10);
            LayoutX1(Children, VW(1) - 10);
            LayoutLinear(Children, Direction.Down, -1, _scrollOffset);
        }
    }
}
