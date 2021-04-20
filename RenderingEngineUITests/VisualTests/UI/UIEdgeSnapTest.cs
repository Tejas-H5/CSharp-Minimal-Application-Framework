using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.DataInput;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;

namespace RenderingEngine.VisualTests.UI
{
    public class UIEdgeSnapTest : EntryPoint
    {
        UIElement _root;
        UIElement _textInputElement;

        UIElement _mouseDriven;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Text input ui element test";

            Window.RenderFrequency = 120;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 0, 0, 1);

            _root = UICreator.CreatePanel(new Color4(1));

            _mouseDriven = UICreator.CreatePanel(new Color4(1))
                .SetNormalizedAnchoring(new Rect2D(0.5f, 0, 1f, 1))
                .SetAbsoluteOffset(10);

            _root.SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
            .SetAbsoluteOffset(new Rect2D(0, 0, 0, 0))
            .AddChildren(
                UICreator.CreatePanel(new Color4(1))
                .AddComponent(new UIEdgeSnapConstraint(_mouseDriven, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Bottom))
                .SetNormalizedAnchoring(new Rect2D(0, 0, 0.5f, 1))
                .SetAbsoluteOffset(10)
                .AddComponent(new UIText("Edge snap driven", new Color4(0, 1)))
                ,
                _mouseDriven
                .AddComponent(new UIText("Mouse driven", new Color4(0, 1)))
            );
        }

        public override void Render(double deltaTime)
        {
            _root.DrawIfVisible(deltaTime);
        }

        public override void Update(double deltaTime)
        {
            _root.Update(deltaTime);

            Rect2D wantedRect = _mouseDriven.Rect;

            wantedRect.Y0 = Input.MouseY;

            _mouseDriven.Rect = wantedRect;
        }

        public override void Resize()
        {
            base.Resize();

            _root.Resize();
        }
    }
}
