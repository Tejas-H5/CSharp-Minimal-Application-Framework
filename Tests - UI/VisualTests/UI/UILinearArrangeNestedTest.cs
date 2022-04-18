namespace MinimalAF.VisualTests.UI {
    [VisualTest(
        description: @"Test that " + nameof(LayoutSplit) + " works when nested.",
        tags: "UI, layout"
    )]
    public class UILinearArrangeNestedTest : Element {
        Element GenerateElement(string text) {
            return new UILinearArrangeTest();
        }

        public UILinearArrangeNestedTest() {
            SetChildren(
                GenerateElement("0 (Press space to toggle layout)   "),
                GenerateElement("1"),
                GenerateElement("2"),
                GenerateElement("3")
            );
        }

        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        int layouting = (int)Direction.Down;

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Space)) {
                layouting = (layouting + 1) % ((int)(Direction.Right + 1));
                Layout();
            }
        }

        public override void OnLayout() {
            LayoutSplit(children, (Direction)layouting);
            LayoutChildren();
        }
    }
}
