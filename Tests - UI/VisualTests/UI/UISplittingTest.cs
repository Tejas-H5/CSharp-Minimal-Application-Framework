namespace MinimalAF.VisualTests.UI {
    public class UISplittingTest : Element {
        public UISplittingTest() {
            Color4 col = new Color4(0, 1);

            this.SetChildren(
                new OutlineRect(col, 5)
                .Anchors(new Rect2D(0, 0, 1, 1))
                .Offsets(25)
                .TopSplit(
                    70,
                    new OutlineRect(col, 4)
                    .SetChildren(new TextElement("TopSplit 70", col)),
                    new OutlineRect(col, 4)
                    .SetChildren()
                    .LeftSplit(
                        200,
                        new OutlineRect(col, 3)
                        .SetChildren(new TextElement("LeftSplit 200", col)),
                        new OutlineRect(col, 3)
                        .BottomSplit(
                            100,
                            new OutlineRect(col, 2)
                            .RightSplit(
                                100,
                                new OutlineRect(col, 1).SetChildren(new TextElement("Nothing", col)),
                                new OutlineRect(col, 2).SetChildren(new TextElement("RightSplit 150", col))
                            ),
                            new OutlineRect(col, 1).SetChildren(new TextElement("BottomSplit 100", col))
                        )
                    )
                )
            );
        }

        public override void OnStart() {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Splitting test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));

            base.OnStart();
        }
    }
}
