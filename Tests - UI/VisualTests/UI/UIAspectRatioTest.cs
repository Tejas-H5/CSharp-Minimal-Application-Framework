namespace MinimalAF.VisualTests.UI {

    class AspectRatioContent : Element {
        TextElement _text;
        Element _child;
        protected override void OnConstruct() {
            SetChildren(
                _child = new OutlineRect(Color4.RGBA(1, 0, 0, 1), 2).SetChildren(
                    _text = new TextElement("4 : 3 Aspect ratio", Color4.RGBA(1, 0, 0, 1))
                )
            );
        }

        public override void OnLayout() {
            Pivot = new OpenTK.Mathematics.Vector2(0.5f, 0.5f);
            LayoutRelativeMargin(10);
            LayoutAspectRatio(4f / 3f, AspectRatioMethod.FitInside);

            _child.LayoutRelativeMargin(0);
            _text.HorizontalAlignment = HorizontalAlignment.Left;
            _text.VerticalAlignment = VerticalAlignment.Bottom;
        }
    }

    public class UIAspectRatioTest : Element {
        Element _container;
        Element _aspectRatio;

        protected override void OnConstruct() {
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
            _container.LayoutRelativeMargin(10);

            base.OnLayout();
        }
    }
}
