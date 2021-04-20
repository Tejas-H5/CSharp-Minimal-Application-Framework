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
    public class UILinearArrangeTest : EntryPoint
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

            UIElement hArrange, vArrange;

            _root.SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
            .SetAbsoluteOffset(new Rect2D(0, 0, 0, 0))
            .AddChildren(
                ///*
                hArrange = UICreator.CreateUIElement(
                    new UIRect(new Color4(0, 0.2f), new Color4(0,1),1),
                    new UILinearArrangement(false, false, 50, 10)
                )
                .SetNormalizedAnchoringY(0,1)
                .SetAbsOffsetsY(10,10)
                .SetNormalizedPositionCenterX(0,0)
                ,
                //*/
                vArrange = UICreator.CreateUIElement(
                    new UIRect(new Color4(0, 0.2f), new Color4(0, 1), 1),
                    new UILinearArrangement(true, true, 50, 10)
                )
                .SetNormalizedAnchoringX(0.75f, 1)
                .SetAbsOffsetsX(10,10)
                .SetNormalizedPositionCenterY(0,0)
            );

            ///*
            for(int i =0; i < 10; i++)
            {
                hArrange.AddChild(
                    UICreator.CreateUIElement(
                        new UIRect(new Color4(0, 0.0f), new Color4(0, 1), 1),
                        new UIText("h" + i.ToString(), new Color4(0, 1))
                    )
                );
            }
            //*/

            for (int i = 0; i < 10; i++)
            {
                vArrange.AddChild(
                    UICreator.CreateUIElement(
                        new UIRect(new Color4(0, 0.0f), new Color4(0, 1), 1),
                        new UIText("v" + i.ToString(), new Color4(0, 1))
                    )
                );
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
