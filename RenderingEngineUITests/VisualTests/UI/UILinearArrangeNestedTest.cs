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
    public class UILinearArrangeNestedTest : EntryPoint
    {
        UIElement _root;
        UIElement _textInputElement;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "xd lmao xdxdxd";

            Window.RenderFrequency = 120;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);

            _root = UICreator.CreateUIElement();
            UIElement fitChildren = UICreator.CreateUIElement(
                new UIGraphicsRaycaster()
                ///new UIFitChildren(false, true, new Rect2D(10,10,10,10))
            );

            _root.AddChild(fitChildren);

            UIElement linearArrange;

            fitChildren
            .AddChildren(
                linearArrange = UICreator.CreateRectOutline(new Color4(0, 1))
                .AddComponent(new UILinearArrangement(true, false, -1, 5))
                .SetNormalizedAnchoringX(0.25f, 0.75f)
                .SetAbsOffsetsX(10,10)
                .SetNormalizedPositionCenterY(1f, 1f)
                .SetAbsPositionSizeY(-10f, 1f)
            );


            for(int i = 0; i < 2; i++)
            {
                UIElement linearArrange2;

                linearArrange
                .AddChildren(
                    linearArrange2 = UICreator.CreateRectOutline(new Color4(0, 1))
                    .AddComponent(new UILinearArrangement(true, false, -1, 5))
                    .SetNormalizedAnchoringX(0,1)
                    .SetAbsOffsetsX(10, 10)
                    .SetNormalizedPositionCenterY(1f, 1f)
                    .SetAbsPositionSizeY(-10f, 1f)
                );

                //*
                for (int j = 0; j < 5; j++)
                {
                    UIElement button;
                    linearArrange2.AddChild(
                        button = UICreator.CreateButton(j.ToString(), "Consolas", 10)
                        .SetNormalizedPositionCenterY(1, 1)
                        .SetAbsPositionSizeY(0, 20)
                    );

                    button.GetComponentOfType<UIMouseListener>().OnMousePressed += (MouseEventArgs e) => {
                        button.SetAbsPositionSizeY(0, button.Rect.Height + 10);
                    };
                }
                //*/
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
