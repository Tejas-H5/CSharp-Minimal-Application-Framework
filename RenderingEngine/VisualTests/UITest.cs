using OpenTK.Mathematics;
using RenderingEngine.UI;
using RenderingEngine.UI.BasicUI;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using Color4 = RenderingEngine.Datatypes.Color4;


namespace RenderingEngine.VisualTests
{
    public class UITest : EntryPoint
    {
        public override void Start()
        {
            Window.Size = new Vector2i(800, 600);
            Window.Title = "UI Test";

            Window.RenderFrequency = 60;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);

            //CTX.SetCurrentFont("Consolas", 24);
            InitUI();
        }


        protected UINode _UIRoot = new UIPanel();
        public UINode UIRoot {
            get {
                return _UIRoot;
            }
        }

        private void InitUI()
        {
            UINode top = GeneratePanel();
            top.SetAnchoringOffset(new Rect2D(0, 0.5f, 1, 1));
            UIRoot.AddChild(top);

            UIPanel bottom = GeneratePanel();
            bottom.SetAnchoringOffset(new Rect2D(0, 0f, 1f, 0.5f));
            UIRoot.AddChild(bottom);

            //Create a golden ratio spiral
            UINode starting = bottom;

            for(int i = 0; i < 5; i++)
            {
                starting = Generate2PanelsHor(starting, false);
                starting = Generate2PanelsVer(starting, false);
                starting = Generate2PanelsHor(starting, true);
                starting = Generate2PanelsVer(starting, true);
            }

            //Add a button
            UIButton button = new UIButton();
            button.SetAnchoringPositionCenter(0,0, 1, 1);
            button.SetRectPositionSize(100, 100, 50, 50);

            //button.SetAnchoring(new Rect2D(0, 0, 1, 1));
            //button.SetRectOffset(new Rect2D(-20, -20, -20, -20));
            button.Text = $"Absolute positioning x={button.RectOffset.X0},y={button.RectOffset.Y0}, " +
                $"\n width={button.RectOffset.X1} and height = {button.RectOffset.Y1}, " +
                $"\n center = {button.Anchoring.X1},{button.Anchoring.Y1}, ";
            button.TextColor = new Color4(0, 0, 0, 1f);
            button.BackgroundRect.Color = new Color4(0.7f, 0.7f, 0.7f, 1f);
            button.BackgroundRect.ClickedColor = new Color4(1f, 1f, 1f, 1f);
            //button.OnClicked += Button_OnClicked;

            top.AddChild(button);
        }

        private void Button_OnClicked()
        {
            Console.WriteLine("Bruh XD");
        }

        private UIPanel Generate2PanelsVer(UINode attatchTo, bool top)
        {
            UIPanel subPanelTop = GeneratePanel();
            subPanelTop.SetAnchoringOffset(new Rect2D(0, 0.5f, 1, 1));
            attatchTo.AddChild(subPanelTop);

            UIPanel subPanelBottom = GeneratePanel();
            subPanelBottom.SetAnchoringOffset(new Rect2D(0, 0f, 1f, 0.5f));
            attatchTo.AddChild(subPanelBottom);

            if (top)
                return subPanelTop;

            return subPanelBottom;
        }

        private UIPanel Generate2PanelsHor(UINode attatchTo, bool left)
        {
            UIPanel subPanelLeft = GeneratePanel();
            subPanelLeft.SetAnchoringOffset(new Rect2D(0f,0f,0.5f,1f));
            attatchTo.AddChild(subPanelLeft);

            UIPanel subPanelRight = GeneratePanel();
            subPanelRight.SetAnchoringOffset(new Rect2D(0.5f, 0f, 1f, 1f));
            attatchTo.AddChild(subPanelRight);

            if (left)
                return subPanelLeft;

            return subPanelRight;
        }

        private static UIPanel GeneratePanel()
        {
            var subPanelLeft = new UIPanel();
            subPanelLeft.BackgroundRect.Color = new Color4(0, 0, 0, 0.1f);
            subPanelLeft.BackgroundRect.HoverColor = new Color4(1, 0, 0, 0.2f);
            subPanelLeft.BackgroundRect.ClickedColor = new Color4(0, 0, 1, 0.2f);
            subPanelLeft.SetRectOffset(new Rect2D(-5, -5, -5, -5));
            return subPanelLeft;
        }

        public override void Render(double deltaTime)
        {
            CTX.Clear();

            _UIRoot.Draw(deltaTime);

            CTX.DrawText("Bruv", Window.Width / 2.0f, Window.Height / 2.0f);

            CTX.Flush();
        }

        public override void Resize()
        {
            CTX.Viewport2D(Window.Width, Window.Height);

            _UIRoot.Resize();

        }

        public override void Update(double deltaTime)
        {
            _UIRoot.Update(deltaTime);
        }
    }
}
