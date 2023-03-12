namespace UIVisualTests {
    [VisualTest(
        description: @"Test that the UI is actually layouting properly at all.",
        tags: "UI, layout"
    )]
    public class UIResizingTest : Element {
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

            SetChildren(
                GenerateElement("0")
            );
        }
    }
}
