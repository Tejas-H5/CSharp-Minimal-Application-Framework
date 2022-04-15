namespace MinimalAF.VisualTests.UI {
	[VisualTest]
    public class UILinearArrangeTest : Element {
        Element _root;
        Element _textInputElement;

        public UILinearArrangeTest() {
            SetChildren(
                GenerateElement("0 (Press space to toggle layout)   "),
                GenerateElement("1"),
                GenerateElement("2"),
                GenerateElement("3")
            );
        }

        Element GenerateElement(string text) {
            return new OutlineRect(Color4.VA(0, 1), 1).SetChildren(
                new TextElement(text, Color4.VA(0, 1), "Consolas", 24, VerticalAlignment.Center, HorizontalAlignment.Center)
            );
        }

        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        int layouting = (int)Direction.Right;

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Space)) {
                layouting = (layouting + 1) % ((int)(Direction.Right + 1));
                Layout();
            }
        }


        public override void OnLayout() {
            LayoutLinear(_children, (Direction)layouting);
            LayoutChildren();
        }
    }
}
