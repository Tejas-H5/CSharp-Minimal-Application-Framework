namespace MinimalAF.VisualTests.UI {
	[VisualTest]
    public class UIResizingTest : Element {
        Element root;
        Element textInputElement;

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

            SetChildren(
                GenerateElement("0")
            );
        }
    }
}
