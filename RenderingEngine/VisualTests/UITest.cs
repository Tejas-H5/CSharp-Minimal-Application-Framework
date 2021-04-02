using OpenTK.Mathematics;
using RenderingEngine.UI;
using RenderingEngine.UI.BasicUI;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;

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

            InitUI();
        }


        protected UIElement _UIRoot = new UIPanel();
        public UIElement UIRoot {
            get {
                return _UIRoot;
            }
        }

        private void InitUI()
        {
            //Create a golden ratio spiral
            UIElement starting = UIRoot;

            for(int i = 0; i < 5; i++)
            {
                starting = Generate2PanelsHor(starting, false);
                starting = Generate2PanelsVer(starting, false);
                starting = Generate2PanelsHor(starting, true);
                starting = Generate2PanelsVer(starting, true);
            }
        }

        private UIPanel Generate2PanelsVer(UIElement attatchTo, bool top)
        {
            UIPanel subPanelTop = GeneratePanel();
            subPanelTop.SetAnchoring(new Rect2D(0, 0.5f, 1, 1));
            attatchTo.AddChild(subPanelTop);

            UIPanel subPanelBottom = GeneratePanel();
            subPanelBottom.SetAnchoring(new Rect2D(0, 0f, 1f, 0.5f));
            attatchTo.AddChild(subPanelBottom);

            if (top)
                return subPanelTop;

            return subPanelBottom;
        }

        private UIPanel Generate2PanelsHor(UIElement attatchTo, bool left)
        {
            UIPanel subPanelLeft = GeneratePanel();
            subPanelLeft.SetAnchoring(new Rect2D(0f,0f,0.5f,1f));
            attatchTo.AddChild(subPanelLeft);

            UIPanel subPanelRight = GeneratePanel();
            subPanelRight.SetAnchoring(new Rect2D(0.5f, 0f, 1f, 1f));
            attatchTo.AddChild(subPanelRight);

            if (left)
                return subPanelLeft;

            return subPanelRight;
        }

        private static UIPanel GeneratePanel()
        {
            var subPanelLeft = new UIPanel();
            subPanelLeft.Color = new Color4(0, 0, 0, 0.1f);
            subPanelLeft.HoverColor = new Color4(1, 0, 0, 0.2f);
            subPanelLeft.ClickedColor = new Color4(0, 0, 1, 0.2f);
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
