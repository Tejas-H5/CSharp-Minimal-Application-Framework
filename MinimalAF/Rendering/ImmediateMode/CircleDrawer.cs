using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering.ImmediateMode
{
	public class CircleDrawer : GeometryDrawer
	{
		ArcDrawer _arcDrawer;

		public CircleDrawer(ArcDrawer drawer)
		{
			_arcDrawer = drawer;
		}


		public void Draw(float x0, float y0, float r, int edges)
		{
			_arcDrawer.Draw(x0, y0, r, 0, MathF.PI * 2, edges);
		}

		public void Draw(float x0, float y0, float r)
		{
			_arcDrawer.Draw(x0, y0, r, 0, MathF.PI * 2);
		}

		public void DrawOutline(float thickness, float x0, float y0, float r, int edges)
		{
			_arcDrawer.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2, edges);
		}

		public void DrawOutline(float thickness, float x0, float y0, float r)
		{
			_arcDrawer.DrawOutline(thickness, x0, y0, r, 0, MathF.PI * 2);
		}
	}
}
