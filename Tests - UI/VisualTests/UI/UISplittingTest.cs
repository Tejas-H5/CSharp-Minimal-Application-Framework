namespace MinimalAF.VisualTests.UI {
    public class UISplittingTest : Element {
        Element _topSplit,
            _leftSplit,
            _rightSplit,
            _bottomSplit;

        public UISplittingTest() {
            // This is a legacy test whose name makes no sense now that the API has changed so much.
            // The functionality should still be there tho

            Color4 col = Color4.VA(0, 1);
            SetChildren(
                _topSplit = new OutlineRect(col, 5).SetChildren(
                    new TextElement("TopSplit 70", col),
                    _leftSplit = new OutlineRect(col, 4).SetChildren(
                        new TextElement("LeftSplit 200", col),
                        _bottomSplit = new OutlineRect(col, 4).SetChildren(
                            new TextElement("BottomSplit 100", col),
                            _rightSplit = new OutlineRect(col, 1).SetChildren(
                                new TextElement("RightSplit 150", col),
                                new OutlineRect(col, 1).SetChildren(
                                    new TextElement("end;", col)
                                )
                            )
                        )
                    )
                )
            );
        }

        public override void OnMount() {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Splitting test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        public override void OnLayout() {
            _topSplit.Children[0].LayoutMargin(10);
            _topSplit.Children[1].LayoutMargin(10);
            _leftSplit.Children[0].LayoutMargin(10);
            _leftSplit.Children[1].LayoutMargin(10);
            _rightSplit.Children[0].LayoutMargin(10);
            _rightSplit.Children[1].LayoutMargin(10);
            _bottomSplit.Children[0].LayoutMargin(10);
            _bottomSplit.Children[1].LayoutMargin(10);

            LayoutElementsSplit(_topSplit.Children, LayoutDirection.Down, 70);
            LayoutElementsSplit(_leftSplit.Children, LayoutDirection.Left, 200);
            LayoutElementsSplit(_rightSplit.Children, LayoutDirection.Right, 100);
            LayoutElementsSplit(_bottomSplit.Children, LayoutDirection.Up, 150);
        }
    }
}
