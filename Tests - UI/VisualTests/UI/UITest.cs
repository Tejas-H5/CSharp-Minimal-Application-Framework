using System;
using System.Collections.Generic;

namespace MinimalAF.VisualTests.UI {
    public class UITest : Element {
        Panel GeneratePanel() {
            return new Panel(Color4.VA(0, 0.1f), Color4.RGBA(0, 0, 1, 0.5f), Color4.RGBA(1, 1, 1, 1));
        }

        float[] goldenRatioSplit = new float[] { 0, 0.38196601125f, 1 };

        List<Element> upSplits = new List<Element>();
        List<Element> leftSplits = new List<Element>();
        List<Element> rightSplits = new List<Element>();
        List<Element> downSplits = new List<Element>();

        protected override void OnConstruct() {
            Element cont = this;

            for (int i = 0; i < 5; i++) {
                Element left = GeneratePanel();
                Element right = GeneratePanel();

                cont.SetChildren(left, right);
                leftSplits.Add(cont);


                Element upper = GeneratePanel();
                Element lower = GeneratePanel();
                right.SetChildren(upper, lower);
                downSplits.Add(right);

                left = GeneratePanel();
                right = GeneratePanel();
                lower.SetChildren(left, right);
                rightSplits.Add(lower);

                upper = GeneratePanel();
                lower = GeneratePanel();
                left.SetChildren(upper, lower);
                upSplits.Add(left);

                cont = upper;
            }

            SetChildren(cont);
        }

        public override void OnLayout() {
            Console.WriteLine("Layout changed");

            Element cont = this;
            for (int i = 0; i < upSplits.Count; i++) {
                leftSplits[i].RelativeRect = new Rect(0, 0, cont.VW(1), cont.VH(1));
                leftSplits[i].LayoutElementsLinear(leftSplits[i].Children, LayoutDirection.Left, goldenRatioSplit, true);

                downSplits[i].RelativeRect = new Rect(0, 0, leftSplits[i].VW(1), leftSplits[i].VH(1));
                downSplits[i].LayoutElementsLinear(downSplits[i].Children, LayoutDirection.Down, goldenRatioSplit, true);

                rightSplits[i].RelativeRect = new Rect(0, 0, downSplits[i].VW(1), downSplits[i].VH(1));
                rightSplits[i].LayoutElementsLinear(rightSplits[i].Children, LayoutDirection.Right, goldenRatioSplit, true);

                upSplits[i].RelativeRect = new Rect(0, 0, rightSplits[i].VW(1), rightSplits[i].VH(1));
                upSplits[i].LayoutElementsLinear(upSplits[i].Children, LayoutDirection.Up, goldenRatioSplit, true);

                cont = upSplits[i];
            }
        }


        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnMount() {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "UI Test";
            w.RenderFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }
    }
}
