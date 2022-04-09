namespace MinimalAF.VisualTests.UI {
    public class UILinearArrangeNestedTest : Element {
        Element _root;
        Element _textInputElement;

        Element GenerateElement(string text) {
            return new UILinearArrangeTest();
        }

        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));

            SetChildren(
                GenerateElement("0 (Press space to toggle layout)   "),
                GenerateElement("1"),
                GenerateElement("2"),
                GenerateElement("3")
            );
        }

        int layouting = (int)LayoutDirection.Down;

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Space)) {
                layouting = (layouting + 1) % ((int)(LayoutDirection.Right + 1));
                Layout();
            }
        }

        public override void OnLayout() {
            LayoutLinear(Children, (LayoutDirection)layouting);
            LayoutChildren();
        }
    }
}
