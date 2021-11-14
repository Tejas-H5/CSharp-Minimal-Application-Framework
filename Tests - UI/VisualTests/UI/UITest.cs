using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF;
using System;

namespace MinimalAF.VisualTests.UI
{
    public class UITest : Container
    {
        Panel GeneratePanel()
        {
            return new Panel(new Color4(0, 0.1f), new Color4(0, 0, 1, 0.1f), new Color4());
        }

        public UITest()
        {
            Container cont = this;

            float[] goldenRatioSplit = new float[] { 0.38196601125f };

            for (int i = 0; i < 5; i++)
            {
                Container left = GeneratePanel();
                Container right = GeneratePanel();
                cont.InColumns(
                    goldenRatioSplit,
                    left, right
                );

                Container upper = GeneratePanel();
                Container lower = GeneratePanel();
                right.InRows(goldenRatioSplit,
                    upper, lower);

                left = GeneratePanel();
                right = GeneratePanel();
                lower.InColumns(
                    goldenRatioSplit,
                    left, right
                );

                upper = GeneratePanel();
                lower = GeneratePanel();
                left.InRows(goldenRatioSplit,
                    upper, lower);

                cont = upper;
            }
        }

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "UI Test";

            w.RenderFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);
        }
    }
}
