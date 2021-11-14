using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF.UI;
using System;

namespace MinimalAF.VisualTests.UI
{
    public class UITest : Container
    {
        public UITest()
        {
            Container cont = this;

            float[] goldenRatioSplit = new float[] { 0.38196601125f };

            for (int i = 0; i < 5; i++)
            {
                Container left = new Panel();
                Container right = new Panel();
                cont.InColumns(
                    goldenRatioSplit,
                    left, right
                );

                Container upper = new Panel();
                Container lower = new Panel();
                right.InRows(goldenRatioSplit,
                    upper, lower);

                left = new Panel();
                right = new Panel();
                lower.InColumns(
                    goldenRatioSplit,
                    left, right
                );

                upper = new Panel();
                lower = new Panel();
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
