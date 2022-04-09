namespace MinimalAF.VisualTests.UI {
    public class UISplittingTest : Element {
        class SplitContainer : Element {
            LayoutDirection dir;
            float splitAmount;

            public SplitContainer(LayoutDirection dir, float splitAmount) {
                this.dir = dir;
                this.splitAmount = splitAmount;
            }

            Panel GeneratePanel(Color4 col) {
                return new Panel(Color4.VA(0, 0.1f), col, Color4.RGBA(0, 1, 0, 0.5f));
            }

            protected override void OnConstruct() {
                SetChildren(
                    GeneratePanel(Color4.RGBA(1, 0, 0, 0.5f)),
                    GeneratePanel(Color4.RGBA(0, 0, 1, 0.5f))
                );
            }

            public override void OnRender() {
                this[0].UseCoordinates();

                SetDrawColor(Color4.VA(0, 1));
                Text("Split " + dir.ToString() + " " + splitAmount.ToString("0.00"),
                    this[0].VW(0.5f), this[0].VH(0.5f),
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center
                );

                UseCoordinates();
            }

            public override void OnLayout() {
                LayoutSplit(Children[0], Children[1], dir, splitAmount);
                LayoutInset(Children, 10);

                LayoutChildren();
            }
        }

        protected override void OnConstruct() {
            Element down = new SplitContainer(LayoutDirection.Down, 70);
            Element left = new SplitContainer(LayoutDirection.Left, 200);
            Element up = new SplitContainer(LayoutDirection.Up, 100);
            Element right = new SplitContainer(LayoutDirection.Right, 150);

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
