using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.UI;

namespace MinimalAF.VisualTests.UI
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

            _root.Anchors(left: 0, bottom: 0, right: 1, top: 1)
            .Offsets(left: 0, bottom: 0, right: 0, top: 0)
            .AddChildren(
                UICreator.CreatePanel(new Color4(1))
                .AddComponents(
                    new UIEdgeSnapConstraint(_mouseDriven, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Bottom),
                    new UIText("Edge snap driven", new Color4(0, 1))
                )
                .Anchors(0, 0, 1f/3f, 1)
                .Offsets(10)
                .AddChildren(
                    UICreator.CreatePanel(new Color4(1))
                    .Offsets(20)
                )
                ,
                _mouseDriven
                .Anchors(1f / 3f, 0, 2f / 3f, 1)
                .Offsets(10)
                .AddComponents(
                    new UIText("Press up/down", new Color4(0, 1)),
                    new MouseBlindThinggyComponent()
                )
                .AddChildren(
                    UICreator.CreatePanel(new Color4(1))
                    .Offsets(20)
                )
                ,
                UICreator.CreatePanel(new Color4(1))
                .Anchors(2f/3f, 0, 1f, 1)
                .Offsets(10)
                .AddComponents(
                    new UIEdgeSnapConstraint(_mouseDriven, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Bottom),
                    new UIText("Edge snap driven", new Color4(0, 1))
                )
                .AddChildren(
                    UICreator.CreatePanel(new Color4(1))
                    .Offsets(20)
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
