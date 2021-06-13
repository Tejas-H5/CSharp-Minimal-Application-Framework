using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.UI;
using System;

namespace MinimalAF.VisualTests.UI
{
    public class UIAspectRatioTest : EntryPoint
    {
        UIElement _root;
        UIElement _textInputElement;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "UIAspectRatioTest";

            Window.RenderFrequency = 120;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(0, 0, 0, 0);

            float indent = 1f / 3f;

            Color4 red = new Color4(1, 0, 0, 1);
            Color4 blue = new Color4(0, 0, 1, 1);
            Color4 white = new Color4(1, 1, 1, 1);

            float aspectRatio = 4f / 3f;

            _root = UICreator.CreateUIElement()
                .AddChildren(
                    CreateRect(white, 2)
                    .Anchors(indent, indent, 1f - indent, 1f - indent)
                    .Offsets(10)
                    .AddChildren(
                        CreateRect(red)
                        .AddComponents(
                            new UIAspectRatioConstraint(aspectRatio),
                            new UIText($"{aspectRatio} aspect ratio", red)
                        )
                        .AnchoredCenter(0.5f,0.5f)
                        .Offsets(10)
                    )
                );
        }

        public UIElement CreateRect(Color4 col, int thickness = 1)
        {
            return UICreator.CreateUIElement(
                    new UIRect(new Color4(0, 0), col, thickness)
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
