namespace MinimalAF.VisualTests.UI {
    public class UITextInputTest : Element {
        public UITextInputTest() {
            Element[] rows = new Element[3];

            for (int i = 0; i < 3; i++) {
                Element[] rowElements = new Element[3];
                for (int j = 0; j < 3; j++) {
                    if (j == 2) {
                        rowElements[j] = new OutlineRect(Color4.VA(0, 1), 1).SetChildren(
                            new TextInput<float>(
                                new TextElement("", Color4.VA(0, 1), "Comic-Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j),
                                new Property<float>(0),
                                (string arg) => {
                                    return float.Parse(arg);
                                }
                            )
                        );
                    } else {
                        rowElements[j] = new OutlineRect(Color4.VA(0, 1), 1).SetChildren(
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

                rows[i] = new Element().SetChildren(
                    rowElements
                );
            }

            SetChildren(
                new UIRootElement().SetChildren(
                    rows
                )
            );
        }


        public override void OnLayout() {
            LayoutElementsLinear(Children, LayoutDirection.Down);

            for (int i = 0; i < Children.Length; i++) {
                Children[i].LayoutElementsLinear(Children[i].Children, LayoutDirection.Right);
            }
        }


        public override void OnUpdate() {
            base.OnUpdate();
        }


        public override void OnMount() {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }
    }
}
