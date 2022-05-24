using MinimalAF;

namespace UIVisualTests {

    class TextInputsUI : Element {
        public TextInputsUI() {
            Element[] rows = new Element[9];

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (j == 2) {
                        rows[i * 3 + j] = new OutlineRect(Color.VA(0, 1), 1).SetChildren(
                            new TextInput<float>(
                                new TextElement("", Color.VA(0, 1), "Comic-Sans", 16, (VAlign)i, (HAlign)j),
                                0,
                                (string arg) => {
                                    return float.Parse(arg);
                                }
                            )
                        );
                    } else {
                        rows[i * 3 + j] = new OutlineRect(Color.VA(0, 1), 1).SetChildren(
                            new TextInput<string>(
                                new TextElement("", Color.VA(0, 1), "Comic-Sans", 16, (VAlign)i, (HAlign)j),
                                "Bruh",
                                (string arg) => {
                                    return arg;
                                }
                            )
                        );
                    }
                }
            }

            SetChildren(rows);
        }

        public override void OnLayout() {
            LayoutInset(Children[0, 3], 0, VH(2f / 3f), 0, 0);
            LayoutLinear(Children[0, 3], Direction.Right, VW(1 / 3f));

            LayoutInset(Children[3, 6], 0, VH(1f / 3f), 0, VH(1f / 3f));
            LayoutLinear(Children[3, 6], Direction.Right, VW(1 / 3f));

            LayoutInset(Children[6, 9], 0, VH(0), 0, VH(2f / 3f));
            LayoutLinear(Children[6, 9], Direction.Right, VW(1 / 3f));

            LayoutInset(Children, 10);

            LayoutChildren();
        }
    }

    [VisualTest(
        description: @"Test that " + nameof(TextInput<object>) + " and of other types work.",
        tags: "UI, layout"
    )]
    public class UITextInputTest : Element {
        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            SetClearColor(Color.RGBA(1, 1, 1, 1));
        }

        Element[] rows;

        public UITextInputTest() {
            SetChildren(new UIRootElement().SetChildren(new TextInputsUI()));
        }
    }
}
