using OpenTK.Mathematics;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using Color4 = RenderingEngine.Datatypes.Color4;

namespace RenderingEngine.VisualTests.UI
{
    public class UITest : EntryPoint
    {
        public override void Start()
        {
            Window.Size = new Vector2i(800, 600);
            Window.Title = "UI Test";

            Window.RenderFrequency = 60;

            CTX.SetClearColor(1, 1, 1, 1);

            //CTX.SetCurrentFont("Consolas", 24);
            InitUI();
        }


        protected UIElement _uiRoot;
        protected UIZStack _zStack;

        public UIElement UIRoot {
            get {
                return _uiRoot;
            }
        }

        UIElement _modal;

        UIElement CreateButton(string s)
        {
            return UICreator.CreateButton(
                s, "Consolas", 12, new Color4(0, 0, 0, 1f), new Color4(0.7f, 1f), new Color4(0.5f, 1f), new Color4(1f)
            );
        }

        private void InitUI()
        {
            _zStack = new UIZStack();

            UIElement top;
            UIElement goldenRatioSpiralContainer;
            UIElement button;

            UIElement bruhButton;
            UIElement closeButton;

            button = UICreator.CreateButton("")
                .SetNormalizedPositionCenter(0, 0, 1, 1)
                .SetAbsPositionSize(200, 200, 50, 50);

            _zStack.AddChildren(
                _uiRoot = GeneratePanel().AddChildren(
                    top = GeneratePanel()
                        .SetNormalizedAnchoring(new Rect2D(0, 0.5f, 1, 1))
                        .AddChildren(
                            button
                            ,
                            GeneratePanel()
                                .SetNormalizedAnchoringX(0, 1)
                                .SetAbsOffsetsX(10, 220)
                                .SetNormalizedPositionCenterY(1, 1)
                                .SetAbsPositionSizeY(-10, 100)
                            ,
                            GeneratePanel()
                                .SetNormalizedPositionCenterX(1, 1)
                                .SetAbsPositionSizeX(-10, 200)
                                .SetNormalizedPositionCenterY(1, 1)
                                .SetAbsPositionSizeY(-10, 200)
                        )
                    ,
                    goldenRatioSpiralContainer = GeneratePanel()
                        .SetNormalizedAnchoring(new Rect2D(0, 0f, 1f, 0.5f))
                    )
                ,
                _modal = UICreator.CreateUIElement(
                    new UIRect(new Color4(0, 0, 0, 0.2f))
                    )
                    .AddChildren(
                        bruhButton = CreateButton("Bruh XD")
                            .SetNormalizedPositionCenter(0.5f, 0.5f)
                            .SetAbsPositionSize(0, 0, 400, 200)
                        ,
                        closeButton = CreateButton("X")
                            .SetNormalizedPositionCenter(1, 1, 1, 1)
                            .SetAbsPositionSize(-10, -10, 40, 40)
                        ,
                        CreateButton("some text")
                            .SetNormalizedPositionCenter(1, 1, 1, 1)
                            .SetAbsPositionSize(-10, -50, 100, 40)
                        ,
                        CreateButton("some more textaoiuoawhiaohwifa")
                            .SetNormalizedPositionCenter(1, 1, 1, 1)
                            .SetAbsPositionSize(-10, -100, 100, 40)
                        ,
                        CreateButton("some more textaoiuoawhiaohwifaa asd foifsd iasfhpdihs pasdhfosh paihsd foafopha d \na weoifo awiefoaewhfoi")
                            .SetNormalizedPositionCenter(1, 1, 1, 1)
                            .SetAbsPositionSize(-10, -150, 100, 40)
                    )
                );


            //Create a golden ratio spiral with UI panels
            UIElement starting = goldenRatioSpiralContainer;
            for (int i = 0; i < 5; i++)
            {
                starting = Generate2PanelsHor(starting, false);
                starting = Generate2PanelsVer(starting, false);
                starting = Generate2PanelsHor(starting, true);
                starting = Generate2PanelsVer(starting, true);
            }

            UIText buttonText = button.GetComponentOfType<UIText>();
            buttonText.Text = $"Absolute positioning x={button.AbsoluteOffset.X0},y={button.AbsoluteOffset.Y0}, " +
                $"\n width={button.AbsoluteOffset.X1} and height = {button.AbsoluteOffset.Y1}, " +
                $"\n center = {button.NormalizedAnchoring.X1},{button.NormalizedAnchoring.Y1}, ";

            button.GetComponentOfType<UIMouseListener>().OnMouseReleased += Button_OnClicked;

            bruhButton.GetComponentOfType<UIMouseListener>().OnMouseReleased += BruhButton_OnClicked;
            closeButton.GetComponentOfType<UIMouseListener>().OnMouseReleased += CloseButton_OnClicked;
        }

        private void CloseButton_OnClicked()
        {
            _modal.IsVisible = false;
        }

        private void BruhButton_OnClicked()
        {
            Console.WriteLine("Bruh XD");
        }

        private void Button_OnClicked()
        {
            ToggleModal();
        }

        private void ToggleModal()
        {
            _modal.IsVisible = !_modal.IsVisible;
        }

        private UIElement Generate2PanelsVer(UIElement attatchTo, bool top)
        {
            UIElement subPanelTop, subPanelBottom;
            attatchTo.AddChildren(
                subPanelTop = GeneratePanel()
                    .SetNormalizedAnchoring(new Rect2D(0, 0.5f, 1, 1))
                ,
                subPanelBottom = GeneratePanel()
                    .SetNormalizedAnchoring(new Rect2D(0, 0f, 1f, 0.5f))
                );

            if (top)
                return subPanelTop;

            return subPanelBottom;
        }

        private UIElement Generate2PanelsHor(UIElement attatchTo, bool left)
        {
            UIElement subPanelLeft, subPanelRight;
            attatchTo.AddChildren(
                subPanelLeft = GeneratePanel()
                    .SetNormalizedAnchoring(new Rect2D(0f, 0f, 0.5f, 1f))
                ,
                subPanelRight = GeneratePanel()
                    .SetNormalizedAnchoring(new Rect2D(0.5f, 0f, 1f, 1f))
                );

            if (left)
                return subPanelLeft;

            return subPanelRight;
        }

        private static UIElement GeneratePanel()
        {
            return UICreator.CreatePanel(
                    color: new Color4(0, 0, 0, 0.1f),
                    hoverColor: new Color4(1, 0, 0, 0.2f),
                    clickedColor: new Color4(0, 0, 1, 0.2f)
                )
                .SetAbsoluteOffset(5);
        }

        public override void Render(double deltaTime)
        {


            _zStack.DrawIfVisible(deltaTime);

            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.DrawCircle(Input.MouseX, Input.MouseY, 10);


        }

        public override void Resize()
        {
            base.Resize();

            _zStack.Resize();
        }

        public override void Update(double deltaTime)
        {
            _zStack.Update(deltaTime);
        }
    }
}
