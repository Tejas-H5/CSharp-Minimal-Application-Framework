using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering.ImmediateMode
{
	public class CircleDrawer
	{
		public CircleDrawer()
		{
		}


		public void Draw(float x0, float y0, float r, int edges)
		{
			CTX.Arc.Draw(x0, y0, r, 0, MathF.PI * 2, edges);
		}

		public void Draw(float x0, float y0, float r)
		{
			CTX.Arc.Draw(x0, y0, r, 0, MathF.PI * 2);
		}

		public void DrawOutline(float thickness, float x0, float y0, float r, int edges)
		{
			CTX.Arc.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2, edges);
		}

		public void DrawOutline(float thickness, float x0, float y0, float r)
		{
			CTX.Arc.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2);
		}
	}
}
