using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering
{
	public class NestingTest : Element
	{
		public NestingTest()
		{
			this.InEvenRows(
				new Element(InEvenColumns(
					new TextTest(),
					new PolylineTest()
				)),
				new Element(InEvenColumns(
					new KeyboardTest(),
					new StencilTest()
				))
			);
		}
	}
}
