using RenderingEngine.Datatypes;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components;
using RenderingEngine.UI.Core;

namespace RenderingEngine.VisualTests.UI
{
    public class UITextInputTest : EntryPoint
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

            _root = UICreator.CreatePanel(new Color4(1));

            _root.SetNormalizedAnchoring(new Rect2D(0,0,1,1))
            .SetAbsoluteOffset(new Rect2D(0,0,0,0))
            .AddChildren(
                _textInputElement = UICreator.CreateUIElement(
                    new UIRect(new Color4(1), new Color4(0), 1),
                    new UIRectHitbox(false),
                    new UIMouseListener(),
                    new UIText("", new Color4(0)),
                    new UIMouseFeedback(new Color4(0.7f), new Color4(0.5f)),
                    new UITextInput()
                )
                .SetNormalizedPositionCenter(0.5f, 0.5f, 0.5f, 0.5f)
                .SetAbsPositionSize(0, 0, 400, 300)
            );
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
