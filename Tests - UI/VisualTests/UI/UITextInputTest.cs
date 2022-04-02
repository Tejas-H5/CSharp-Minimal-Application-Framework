namespace MinimalAF.VisualTests.UI {
    public class UITextInputTest : Element {
        protected override void OnConstruct() {
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
