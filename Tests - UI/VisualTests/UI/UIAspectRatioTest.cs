namespace MinimalAF.VisualTests.UI {

    class AspectRatioContent : Element {
        TextElement _text;
        Element _child;
        public AspectRatioContent() {
            SetChildren(
                _child = new OutlineRect(Color4.RGBA(1, 0, 0, 1), 2).SetChildren(
                    _text = new TextElement("4 : 3 Aspect ratio", Color4.RGBA(1, 0, 0, 1))
                )
            );
        }

        public override void OnLayout() {
            Pivot = new OpenTK.Mathematics.Vector2(0.5f, 0.5f);
            LayoutMargin(10);
            LayoutAspectRatio(4f / 3f, AspectRatioMethod.FitInside);

            _child.LayoutMargin(0);
            _text.HorizontalAlignment = HorizontalAlignment.Left;
            _text.VerticalAlignment = VerticalAlignment.Bottom;
        }
    }

    public class UIAspectRatioTest : Element {
        Element _container;
        Element _aspectRatio;

        public UIAspectRatioTest() : base() {
            SetChildren(
                _container = new OutlineRect(Color4.RGBA(1, 1, 1, 1), 2)
                .SetChildren(
                    new AspectRatioContent()
                )
            );
        }

        public override void OnMount() {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "UIAspectRatioTest";
            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(0, 0, 0, 0));
        }

        public override void OnLayout() {
            _container.LayoutMargin(10);
        }
    }
}
