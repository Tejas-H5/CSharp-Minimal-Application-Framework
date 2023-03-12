using MinimalAF;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RenderingEngineVisualTests
{
	[VisualTest(
        description: @"Test that the poly-line drawing functionality is working.",
        tags: "2D, polyline"
    )]
	public class PolylineTest : IRenderable {
        Queue<Vector2> points = new Queue<Vector2>();
		Queue<double> times = new Queue<double>();

		Vector2 linePoint, linePointDragStart;
		bool dragStarted;
		float radius = 50;

		double timer = 0;

        public PolylineTest(FrameworkContext ctx) {
            linePoint = new Vector2(ctx.VW * 0.5f, ctx.VH * 0.5f);
        }


        //  public override void OnMount() {

        //	w.Size = (800, 600);
        //	w.Title = "Mouse test";

        //	w.RenderFrequency = 120;
        //	//w.UpdateFrequency = 120; 20;

        //	SetClearColor(Color.RGBA(1, 1, 1, 1));
        //	SetFont("Consolas", 16);

        //	// TODO: get this working

        //	//_linePoint = new Vector2(400, 300);
        //}


        public void Render(FrameworkContext ctx) {
            var w = ctx.Window;

            timer += Time.DeltaTime;

            points.Enqueue(linePoint);
            times.Enqueue(timer);

            // remove points after 0.5 seconds
            if (timer - times.Peek() > 0.5f) {
                points.Dequeue();
                times.Dequeue();
            }

            if (Intersections.IsInsideCircle(w.MouseX, w.MouseY, linePoint.X, linePoint.Y, radius)) {
                ctx.SetDrawColor(1, 0, 0, 0.5f);
                if (w.MouseStartedDragging) {
                    linePointDragStart = linePoint;
                    dragStarted = true;
                }
            } else {
                ctx.SetDrawColor(0, 0, 1, 0.5f);
            }

            if (!w.MouseCurrentlyDragging) {
                dragStarted = false;
            }

            if (dragStarted && w.MouseCurrentlyDragging) {
                linePoint = new Vector2(
                    MathHelper.Clamp(linePointDragStart.X + w.MouseDragDeltaX, ctx.VW * 0.25f, ctx.VW * 0.75f),
                    MathHelper.Clamp(linePointDragStart.Y + w.MouseDragDeltaY, ctx.VH * 0.25f, ctx.VH * 0.75f)
                );
            }

            ctx.SetDrawColor(0, 0, 0, 1);
            ctx.DrawText(
                "Mouse test (And polyline test) - Drag that point with your mouse",
                0, ctx.VH,
                HAlign.Left, VAlign.Top
            );

            ctx.DrawRectOutline(5, new Rect(ctx.VW * 0.25f, ctx.VH * ctx.VH * 0.25f, ctx.VW * 0.75f, ctx.VH * 0.75f));

            if (points.Count < 2)
                return;

            int i = 0;
            foreach (Vector2 p in points) {
                if (i == 0) {
                    ctx.StartPolyLine(p.X, p.Y, radius, CapType.Circle);
                } else if (i == points.Count - 1) {
                    ctx.EndPolyLine(p.X, p.Y);
                } else {
                    ctx.ContinuePolyLine(p.X, p.Y);
                }

                i++;
            }
        }
	}
}
