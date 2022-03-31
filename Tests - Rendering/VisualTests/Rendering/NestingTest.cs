using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering
{
	public class NestingTest : Element
	{
        Element el1;
        Element el2;
        Element el3;
        Element el4;
        public NestingTest()
		{
            SetChildren(
                el1 = new TextTest(),
                el2 = new PolylineTest(),
                el3 = new KeyboardTest(),
                el4 = new StencilTest()
            );
		}

        public override void OnLayout() {
            el1.RelativeRect = new Rect(0, 0, VW(0.5f), VH(0.5f));
            el2.RelativeRect = new Rect(0, VH(0.5f), VW(0.5f), VH(0.5f));
            el3.RelativeRect = new Rect(VW(0.5f), 0, VW(0.5f), VH(0.5f));
            el4.RelativeRect = new Rect(VW(0.5f), VH(0.5f), VW(0.5f), VH(0.5f));
        }
    }
}
