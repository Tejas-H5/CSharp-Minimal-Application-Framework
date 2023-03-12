namespace UIVisualTests {
    [VisualTest(
        description: @"Test that " + nameof(LayoutLinear) + " works.",
        tags: "UI, layout"
    )]
    public class UILinearArrangeTest : Element {
        public UILinearArrangeTest() {
            SetChildren(
                GenerateElement("0"),
                GenerateElement("1"),
                GenerateElement("2"),
                GenerateElement("3")
            );
        }

        Element GenerateElement(string text) {
            return new OutlineRect(Color4.VA(0, 1), 1).SetChildren(
                new TextElement(text, Color4.VA(0, 1), "Consolas", 24, VAlign.Center, HAlign.Center)
            );
        }

        public override void OnMount() {

            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        Direction _layouting = Direction.Right;

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Space)) {
                _layouting = _layouting + 1;
                if(_layouting > Direction.Right) {
                    _layouting = 0;
                }

                ((TextElement)Children[0][0]).String = "Layout: " + _layouting.ToString();

                TriggerLayoutRecalculation();
            }
        }


        public override void OnLayout() {
            LayoutLinear(Children, _layouting, VDir(_layouting, 1f / Children.Length));

            LayoutChildren();
        }
    }
}
