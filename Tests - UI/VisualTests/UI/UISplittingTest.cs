using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.UI.Components.MouseInput;
using MinimalAF.UI.Components.Visuals;
using MinimalAF.UI.Core;

namespace MinimalAF.VisualTests.UI
{
    public class UISplittingTest : EntryPoint
    {
        UIElement _root;

        UIElement CreateRect(int thicnkess)
        {
            return UICreator.CreateUIElement(
                new UIRect(new Color4(0, 0), new Color4(0,1), thicnkess)
            );
        }

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
                CreateRect(5).TopSplit( 70,
                    CreateTextElement("Top-Split 70")
                    ,
                    CreateRect(4).LeftSplit(200,
                        CreateTextElement("Left-Split 200")
                        ,
                        CreateRect(3).BottomSplit(100,
                            CreateRect(2).RightSplit(150,
                                CreateRect(1).AddChild(
                                    CreateTextElement("Null")
                                )
                                ,
                                CreateRect(1).AddChild(
                                    CreateTextElement("Right-Split 150")
                                )
                            )
                            ,
                            CreateTextElement("Bottom-Split 100")
                        )
                    )
                )
            );
        }


        UIElement CreateTextElement(string s)
        {
            Color4 textColor = new Color4(0, 1);

            return UICreator.CreateUIElement().AddChild(
                UICreator.CreateUIElement(
                    new UIText(s, textColor)
                )
                .Offsets(5)
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
