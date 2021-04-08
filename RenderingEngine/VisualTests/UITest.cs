using OpenTK.Mathematics;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using Color4 = RenderingEngine.Datatypes.Color4;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Components;
using RenderingEngine.UI;

namespace RenderingEngine.VisualTests
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


        private void InitUI()
        {
            _zStack = new UIZStack();

            UIElement top;
            UIElement goldenRatioSpiralContainer;
            UIElement button;

            UIElement bruhButton;
            UIElement closeButton;

            _zStack.AddChildren(
                _uiRoot = GeneratePanel().AddChildren(
                    top = GeneratePanel()
                        .SetAnchoringOffset(new Rect2D(0, 0.5f, 1, 1))
                        .AddChildren(
                            button = UICreator.CreateButton("")
                                .SetAnchoringPositionCenter(0, 0, 1, 1)
                                .SetRectPositionSize(200, 200, 50, 50)
                            ,
                            GeneratePanel()
                                .SetAnchoringOffsetsX(0, 1)
                                .SetRectOffsetsX(5, 220)
                                .SetAnchoringPositionCenterY(1,1)
                                .SetPositionSizeY(-10, 100)
                            ,
                            GeneratePanel()
                                .SetAnchoringPositionCenterX(1, 1)
                                .SetPositionSizeX(-10, 200)
                                .SetAnchoringPositionCenterY(1, 1)
                                .SetPositionSizeY(-10, 200)
                        )
                    ,
                    goldenRatioSpiralContainer = GeneratePanel()
                        .SetAnchoringOffset(new Rect2D(0, 0f, 1f, 0.5f))
                    )
                ,
                _modal = UICreator.CreateUIElement(
                    new UIRect(new Color4(0, 0, 0, 0.2f))
                    )
                    .AddChildren(
                        bruhButton = UICreator.CreateButton(
                            "Bruh XD", new Color4(0, 0, 0, 1f), new Color4(0.7f, 1f), new Color4(0.5f, 1f), new Color4(1f)
                            )
                            .SetAnchoringPositionCenter(0.5f, 0.5f)
                            .SetRectPositionSize(0, 0, 400, 200)
                        ,
                        closeButton = UICreator.CreateButton(
                            "X", new Color4(0, 0, 0, 1f), new Color4(0.7f, 1f), new Color4(0.5f, 1f), new Color4(1f)
                            )
                            .SetAnchoringPositionCenter(1, 1, 1, 1)
                            .SetRectPositionSize(-10, -10, 40, 40)
                        ,
                        UICreator.CreateButton(
                            "someText", new Color4(0, 0, 0, 1f), new Color4(0.7f, 1f), new Color4(0.5f, 1f), new Color4(1f)
                            )
                            .SetAnchoringPositionCenter(1, 1, 1, 1)
                            .SetRectPositionSize(-10, -50, 100, 40)
                        ,
                        UICreator.CreateButton(
                            "some more textaoiuoawhiaohwifa", new Color4(0, 0, 0, 1f), new Color4(0.7f, 1f), new Color4(0.5f, 1f), new Color4(1f)
                            )
                            .SetAnchoringPositionCenter(1, 1, 1, 1)
                            .SetRectPositionSize(-10, -100, 100, 40)
                        ,
                        UICreator.CreateButton(
                            "some more textaoiuoawhiaohwifaa asd foifsd iasfhpdihs pasdhfosh paihsd foafopha d \na weoifo awiefoaewhfoi", new Color4(0, 0, 0, 1f), new Color4(0.7f, 1f), new Color4(0.5f, 1f), new Color4(1f)
                            )
                            .SetAnchoringPositionCenter(1, 1, 1, 1)
                            .SetRectPositionSize(-10, -150, 100, 40)
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
            buttonText.Text = $"Absolute positioning x={button.RectOffset.X0},y={button.RectOffset.Y0}, " +
                $"\n width={button.RectOffset.X1} and height = {button.RectOffset.Y1}, " +
                $"\n center = {button.Anchoring.X1},{button.Anchoring.Y1}, ";

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
                    .SetAnchoringOffset(new Rect2D(0, 0.5f, 1, 1))
                ,
                subPanelBottom = GeneratePanel()
                    .SetAnchoringOffset(new Rect2D(0, 0f, 1f, 0.5f))
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
                    .SetAnchoringOffset(new Rect2D(0f, 0f, 0.5f, 1f))
                ,
                subPanelRight = GeneratePanel()
                    .SetAnchoringOffset(new Rect2D(0.5f, 0f, 1f, 1f))
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
                .SetRectOffset(5);
        }

        public override void Render(double deltaTime)
        {
            CTX.Clear();

            _zStack.DrawIfVisible(deltaTime);

            CTX.SetDrawColor(0, 0, 0, 1);
            CTX.DrawCircle(Input.MouseX, Input.MouseY, 10);

            CTX.Flush();
        }

        public override void Resize()
        {
            CTX.Viewport2D(Window.Width, Window.Height);

            _zStack.Resize();
        }

        public override void Update(double deltaTime)
        {
            _zStack.Update(deltaTime);
        }
    }
}
