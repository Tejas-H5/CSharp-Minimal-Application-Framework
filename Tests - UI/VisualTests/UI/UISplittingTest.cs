namespace MinimalAF.VisualTests.UI {
	[VisualTest]
    public class UISplittingTest : Element {
        class SplitContainer : Element {
            Direction dir;
            float splitAmount;

            Panel GeneratePanel(Color4 col) {
                return new Panel(Color4.VA(0, 0.1f), col, Color4.RGBA(0, 1, 0, 0.5f));
            }

            public SplitContainer(Direction dir, float splitAmount) {
                this.dir = dir;
                this.splitAmount = splitAmount;

                SetChildren(
                    GeneratePanel(Color4.RGBA(1, 0, 0, 0.5f)),
                    GeneratePanel(Color4.RGBA(0, 0, 1, 0.5f))
                );
            }

            public override void OnRender() {
                this[0].ResetCoordinates();

                SetDrawColor(Color4.VA(0, 1));
                Text("Split " + dir.ToString() + " " + splitAmount.ToString("0.00"),
                    this[0].VW(0.5f), this[0].VH(0.5f),
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center
                );

                ResetCoordinates();
            }

            public override void OnLayout() {
                LayoutTwoSplit(children[0], children[1], dir, splitAmount);
                LayoutInset(children, 10);

                LayoutChildren();
            }
        }

        public UISplittingTest() {
            Element down = new SplitContainer(Direction.Down, 70);
            Element left = new SplitContainer(Direction.Left, 200);
            Element up = new SplitContainer(Direction.Up, 100);
            Element right = new SplitContainer(Direction.Right, 150);

            down[1].SetChildren(left);
            left[1].SetChildren(up);
            up[1].SetChildren(right);

            SetChildren(
                new UIRootElement().SetChildren(
                    down
                )
            );
        }

        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "Splitting test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }
    }
}
