using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.UI;
using MinimalAF.UI.Components.AutoResizing;
using MinimalAF.UI.Components.DataInput;
using MinimalAF.UI.Components.MouseInput;
using MinimalAF.UI.Components.Visuals;
using MinimalAF.UI.Core;
using MinimalAF.UI.Property;
using System;

namespace MinimalAF.VisualTests.UI
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
                .PosSize(0,0,10,10)
                .AddChildren(
                    UICreator.CreateUIElement(
                        new UIRect(new Color4(0,0), new Color4(0, 1), 1),
                        new UIText("margin 5", new Color4(0,1), VerticalAlignment.Center, HorizontalAlignment.Center)
                    )
                    .AnchoredPosCenter(0.5f,0.5f)
                    .PosSize(0,0,100,30)
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
