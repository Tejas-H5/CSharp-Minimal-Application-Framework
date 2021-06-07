using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.UI;

namespace MinimalAF.VisualTests.UI
{
    public class UITextNumberInputTest : EntryPoint
    {
        UIElement _root;
        UIElement _textInputElement;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Text input ui element test";

            Window.RenderFrequency = 120;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 0, 0, 1);

            _root = UICreator.CreatePanel(new Color4(1))
                .Anchors(new Rect2D(0, 0, 1, 1))
                .Offsets(new Rect2D(0, 0, 0, 0))
                .AddComponent(new UIGraphicsRaycaster());

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float size = 300;
                    _root.AddChildren(
                        _textInputElement = UICreator.CreateUIElement(
                            new UIRect(new Color4(1), new Color4(0), 1),
                            new UIRectHitbox(false),
                            new UIMouseListener(),
                            new UIText("", new Color4(0), "Comic Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j),
                            new UIMouseFeedback(new Color4(0.7f), new Color4(0.5f)),
                            new UITextFloatInput(new FloatProperty(10, -30, 29.9f, 0.5f), false)
                        )
                        .AnchoredPosCenter(0.5f, 0.5f, 0.5f, 0.5f)
                        .PosSize((i - 1) * size, (j - 1) * size, size - 10, size - 10)
                    );
                }
            }
        }

        public override void Render(double deltaTime)
        {
            _root.DrawIfVisible(deltaTime);
        }

        public override void Update(double deltaTime)
        {
            _root.Update(deltaTime);
        }

        public override void Resize()
        {
            base.Resize();

            _root.Resize();
        }
    }
}
