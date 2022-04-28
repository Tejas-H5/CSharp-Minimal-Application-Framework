using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering
{
	[VisualTest(
        description: @"Test that multiple elements can be rendered side by side without interfering with one another.
This was the one of the main reasons why I am making this in the first place, and not just using Processing",
        tags: "2D, nesting"
    )]
	public class NestingTest : Element
	{
        Element el1;
        Element el2;
        Element el3;
        Element el4;

        public NestingTest()
		{
            el1 = new TextTest();
            el2 = new PolylineTest();
            el3 = new PerspectiveCameraTest();
            el4 = new StencilTest();

            SetChildren(el1, el2, el3, el4);
		}

        public override void OnLayout() {
            el1.RelativeRect = new Rect(0, 0, VW(0.5f), VH(0.5f));
            el2.RelativeRect = new Rect(0, VH(0.5f), VW(0.5f), VH(1f));
            el3.RelativeRect = new Rect(VW(0.5f), 0, VW(1f), VH(0.5f));
            el4.RelativeRect = new Rect(VW(0.5f), VH(0.5f), VW(1), VH(1));

            LayoutChildren();
        }
    }
}
        