using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF.UI;

namespace MinimalAF.VisualTests.UI
{
    public class UISplittingTest : Container
    {
        public UISplittingTest()
        {
            Color4 col = new Color4(1, 0);

            this.SetChildren(
                new OutlineRect(col, 5)
                .Anchors(new Rect2D(0, 0, 1, 1))
                .Offsets(10)
                .TopSplit(
                    70,
                    new OutlineRect(col, 4, new TextElement("TopSplit 70", col)),
                    new OutlineRect(col, 4)
                    .Offsets(10)
                    .LeftSplit(
                        200,
                        new OutlineRect(col, 3, new TextElement("LeftSplit 200", col)),
                        new OutlineRect(col, 3)
                        .Offsets(10)
                        .RightSplit(
                            150,
                            new OutlineRect(col, 2, new TextElement("RightSplit 150", col)),
                            new OutlineRect(col, 2)
                            .Offsets(10)
                            .BottomSplit(
                                100,
                                new OutlineRect(col, 1, new TextElement("BottomSplit 100", col)),
                                new OutlineRect(col, 1, new TextElement("Nothing", col))
                            )
                        )
                    )
                )
            );
        }

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120; 

            CTX.SetClearColor(1, 0, 0, 1);
        }
    }
}
