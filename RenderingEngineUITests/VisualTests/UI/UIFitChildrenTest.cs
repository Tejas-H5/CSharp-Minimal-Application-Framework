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
using System;

namespace RenderingEngine.VisualTests.UI
{
    public class UIFitChildrenTest : EntryPoint
    {
        UIElement _root;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Text input ui element test";

            Window.RenderFrequency = 120;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 0, 0, 1);

            _root = UICreator.CreatePanel(new Color4(1))
                .AddComponent(new UIGraphicsRaycaster());

            UIElement autoResizingElement;

            _root.Anchors(new Rect2D(0, 0, 1, 1))
            .Offsets(new Rect2D(0, 0, 0, 0))
            .AddChildren(
                autoResizingElement = UICreator.CreateUIElement(
                    new UIRect(new Color4(1,0,0,0.2f), new Color4(0,1), 1),
                    new UIFitChildren(true, true)
                )
                .AnchoredPosCenter(0.5f, 0.5f, 0.5f,0.5f)
                .AddChildren(
                    UICreator.CreateUIElement(
                        new UIRect(new Color4(0,0), new Color4(0, 1), 1),
                        new UIText("margin 5", new Color4(0,1), VerticalAlignment.Center, HorizontalAlignment.Center)
                    )
                    .Offsets(5)
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
