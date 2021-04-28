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
    class MouseBlindThinggyComponent : UIComponent
    {
        Rect2D _wantedRect;

        float yPos = 0;

        public override void Update(double deltaTime)
        {
            if (Input.IsKeyPressed(KeyCode.Down))
            {
                yPos += -10;
                _parent.SetDirty();
            }
            else if (Input.IsKeyPressed(KeyCode.Up))
            {
                yPos += 10;
                _parent.SetDirty();
            }
        }

        public override void OnResize()
        {
            _wantedRect = _parent.Rect;

            _wantedRect.Y0 += yPos;

            _parent.Rect = _wantedRect;
        }

        public override UIComponent Copy()
        {
            return new MouseBlindThinggyComponent();
        }
    }


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

            _mouseDriven = UICreator.CreatePanel(new Color4(1));

            _root.SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
            .SetAbsoluteOffset(new Rect2D(0, 0, 0, 0))
            .AddChildren(
                UICreator.CreatePanel(new Color4(1))
                .AddComponent(new UIEdgeSnapConstraint(_mouseDriven, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Bottom))
                .SetNormalizedAnchoring(new Rect2D(0, 0, 1f/3f, 1))
                .SetAbsoluteOffset(10)
                .AddComponent(new UIText("Edge snap driven", new Color4(0, 1)))
                .AddChildren(
                    UICreator.CreatePanel(new Color4(1))
                    .SetAbsoluteOffset(20)
                )
                ,
                _mouseDriven
                .SetNormalizedAnchoring(new Rect2D(1f / 3f, 0, 2f / 3f, 1))
                .SetAbsoluteOffset(10)
                .AddComponent(new UIText("Press up/down", new Color4(0, 1)))
                .AddComponent(new MouseBlindThinggyComponent())
                .AddChildren(
                    UICreator.CreatePanel(new Color4(1))
                    .SetAbsoluteOffset(20)
                )
                ,
                UICreator.CreatePanel(new Color4(1))
                .AddComponent(new UIEdgeSnapConstraint(_mouseDriven, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Bottom))
                .SetNormalizedAnchoring(new Rect2D(2f/3f, 0, 1f, 1))
                .SetAbsoluteOffset(10)
                .AddComponent(new UIText("Edge snap driven", new Color4(0, 1)))
                .AddChildren(
                    UICreator.CreatePanel(new Color4(1))
                    .SetAbsoluteOffset(20)
                )
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
