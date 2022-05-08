using MinimalAF.Rendering;
using MinimalAF.Util;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
	[VisualTest(
        description: @"Test that the poly-line drawing functionality is working.",
        tags: "2D, polyline"
    )]
	public class PolylineTest : Element
	{
		Queue<Vector2> points = new Queue<Vector2>();
		Queue<double> times = new Queue<double>();

		Vector2 linePoint, linePointDragStart;
		bool dragStarted;
		float radius = 50;

		double timer = 0;

		public override void OnMount(Window w)
		{
			
			w.Size = (800, 600);
			w.Title = "Mouse test";

			w.RenderFrequency = 120;
			//w.UpdateFrequency = 120; 20;

			SetClearColor(Color4.RGBA(1, 1, 1, 1));
			SetFont("Consolas", 16);

			// TODO: get this working
			linePoint = (0,0);
			//_linePoint = new Vector2(400, 300);
		}


		public override void OnUpdate()
		{
			timer += Time.DeltaTime;

            if (linePoint == new Vector2(0, 0)) {
                linePoint = new Vector2(VW(0.5f), VH(0.5f));
            }

			points.Enqueue(linePoint);
			times.Enqueue(timer);

			if (timer - times.Peek() > 0.5f)
			{
				points.Dequeue();
				times.Dequeue();
			}

			if (Intersections.IsInsideCircle(MouseX, MouseY, linePoint.X, linePoint.Y, radius) && MouseStartedDragging)
			{
				linePointDragStart = linePoint;
				dragStarted = true;
			}

			if(MouseStoppedDraggingAnywhere)
			{
				dragStarted = false;
			}

			if (dragStarted)
			{
				linePoint = new Vector2(
					MathUtilF.Clamp(linePointDragStart.X + MouseDragDeltaX, VW(0.25f), VW(0.75f)),
					MathUtilF.Clamp(linePointDragStart.Y + MouseDragDeltaY, VH(0.25f), VH(0.75f))
				);
			}
		}

		public override void OnRender()
		{
			if (points.Count < 2)
				return;

			SetDrawColor(0,0,0,1);
			DrawText("Mouse test (And polyline test) - Drag that point with your mouse", 0, Height, HorizontalAlignment.Left, VerticalAlignment.Top);

			DrawRectOutline(5, new Rect(VW(0.25f), VH(0.25f), VW(0.75f), VH(0.75f)));

			SetDrawColor(0, 0, 1, 0.5f);

			int i = 0;
			foreach (Vector2 p in points)
			{
				if (i == 0)
				{
					StartPolyLine(p.X, p.Y, radius, CapType.Circle);
				}
				else if (i == points.Count - 1)
				{
					EndPolyLine(p.X, p.Y);
				}
				else
				{
					ContinuePolyLine(p.X, p.Y);
				}

				i++;
			}
		}
	}
}
