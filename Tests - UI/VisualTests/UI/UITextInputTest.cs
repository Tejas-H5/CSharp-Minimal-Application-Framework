namespace MinimalAF.VisualTests.UI {

    class TextInputsUI : Element {
        public TextInputsUI() {
            Element[] rows = new Element[9];

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (j == 2) {
                        rows[i * 3 + j] = new OutlineRect(Color4.VA(0, 1), 1).SetChildren(
                            new TextInput<float>(
                                new TextElement("", Color4.VA(0, 1), "Comic-Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j),
                                new Property<float>(0),
                                (string arg) => {
                                    return float.Parse(arg);
                                }
                            )
                        );
                    } else {
                        rows[i * 3 + j] = new OutlineRect(Color4.VA(0, 1), 1).SetChildren(
                            new TextInput<string>(
                                new TextElement("", Color4.VA(0, 1), "Comic-Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j),
                                new Property<string>(""),
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
            LayoutInset(Children.Slice(0, 3), 0, VH(2f / 3f), 0, 0);
            LayoutLinear(Children.Slice(0, 3), LayoutDirection.Right);

            LayoutInset(Children.Slice(3, 6), 0, VH(1f / 3f), 0, VH(1f / 3f));
            LayoutLinear(Children.Slice(3, 6), LayoutDirection.Right);

            LayoutInset(Children.Slice(6, 9), 0, VH(0), 0, VH(2f / 3f));
            LayoutLinear(Children.Slice(6, 9), LayoutDirection.Right);

            LayoutInset(Children, 10);

            LayoutChildren();

        }
    }

    public class UITextInputTest : Element {
        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        Element[] rows;

        public UITextInputTest() {
            SetChildren(new UIRootElement().SetChildren(new TextInputsUI()));
        }
    }
}
