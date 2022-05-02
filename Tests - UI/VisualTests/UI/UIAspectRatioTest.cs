namespace MinimalAF.VisualTests.UI {
    class AspectRatioContent : Element {
        TextElement text;
        Element child;
        public AspectRatioContent() {
            SetChildren(
                child = new OutlineRect(Color4.RGBA(1, 0, 0, 1), 2).SetChildren(
                    text = new TextElement("4 : 3 Aspect ratio", Color4.RGBA(1, 0, 0, 1))
                )
            );
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Left) || KeyPressed(KeyCode.Right) || KeyPressed(KeyCode.Up) || KeyPressed(KeyCode.Down)) {
                if (KeyPressed(KeyCode.Left)) {
                    Pivot.X -= 0.5f;
                }

                if (KeyPressed(KeyCode.Right)) {
                    Pivot.X += 0.5f;
                }

                if (KeyPressed(KeyCode.Up)) {
                    Pivot.Y += 0.5f;
                }

                if (KeyPressed(KeyCode.Down)) {
                    Pivot.Y -= 0.5f;
                }

                TriggerLayoutRecalculation();
            }
        }

        public override void OnRender() {
            SetDrawColor(Color4.VA(0, 1));

            string text = "Pivot: {" + Pivot.X.ToString("0.00") + ", " + Pivot.Y.ToString("0.00") + "}\n" +
                "(use arrow keys to move)";

            DrawText(text, VW(0.5f), VH(0.5f), HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public override void OnLayout() {
            LayoutAspectRatio(child, 4f / 3f, AspectRatioMethod.FitInside);
            LayoutInset(child, 10);

            LayoutInset(child, 0);
            text.HorizontalAlignment = HorizontalAlignment.Left;
            text.VerticalAlignment = VerticalAlignment.Bottom;

            LayoutChildren();
        }
    }

	[VisualTest(
        description: @"Test the aspect ratio layouting function",
        tags: "UI, layout"
    )]
    public class UIAspectRatioTest : Element {
        Element container;

        public UIAspectRatioTest() {
            SetChildren(
                container = new OutlineRect(Color4.RGBA(1, 1, 1, 1), 2)
                .SetChildren(
                    new AspectRatioContent()
                )
            );
        }

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "UIAspectRatioTest";
            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        public override void OnLayout() {
            LayoutInset(container, 10);
            LayoutChildren();
        }
    }
}
