using MinimalAF.Rendering;
using MinimalAF.Util;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
	public class PolylineTest : Element
	{
		Queue<PointF> _points = new Queue<PointF>();
		Queue<double> _times = new Queue<double>();

		PointF _linePoint, _linePointDragStart;
		bool _dragStarted;
		float _radius = 50;

		double timer = 0;

		public override void OnMount()
		{
			Window w = GetAncestor<Window>();
			w.Size = (800, 600);
			w.Title = "Mouse test";

			w.RenderFrequency = 120;
			//w.UpdateFrequency = 120; 20;

			SetClearColor(Color4.RGBA(1, 1, 1, 1));
			SetFont("Consolas", 16);

			// TODO: get this working
			_linePoint = new PointF(VW(0.5f), VH(0.5f));
			//_linePoint = new PointF(400, 300);
		}


		public override void OnUpdate()
		{
			timer += Time.DeltaTime;

			_points.Enqueue(_linePoint);
			_times.Enqueue(timer);

			if (timer - _times.Peek() > 0.5f)
			{
				_points.Dequeue();
				_times.Dequeue();
			}

			if (Intersections.IsInsideCircle(MouseX, MouseY, _linePoint.X, _linePoint.Y, _radius) && MouseStartedDragging)
			{
				_linePointDragStart = _linePoint;
				_dragStarted = true;
			}

			if(MouseFinishedDragging)
			{
				_dragStarted = false;
			}

			if (_dragStarted)
			{
				_linePoint = new PointF(
					MathUtilF.Clamp(_linePointDragStart.X + MouseDragDeltaX, VW(0.25f), VW(0.75f)),
					MathUtilF.Clamp(_linePointDragStart.Y + MouseDragDeltaY, VH(0.25f), VH(0.75f))
				);
			}
		}

		public override void OnRender()
		{
			if (_points.Count < 2)
				return;

			SetDrawColor(0,0,0,1);
			Text("Mouse test (And polyline test) - Drag that point with your mouse", 0, Height, HorizontalAlignment.Left, VerticalAlignment.Top);

			RectOutline(5, new Rect(VW(0.25f), VH(0.25f), VW(0.75f), VH(0.75f)));

			SetDrawColor(0, 0, 1, 0.5f);

			int i = 0;
			foreach (PointF p in _points)
			{
				if (i == 0)
				{
					StartPolyLine(p.X, p.Y, _radius, CapType.Circle);
				}
				else if (i == _points.Count - 1)
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
