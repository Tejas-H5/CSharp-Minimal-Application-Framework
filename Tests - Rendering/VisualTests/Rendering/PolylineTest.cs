using MinimalAF;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RenderingEngineVisualTests {
    public class PolylineTest : IRenderable {
        Queue<Vector2> points = new Queue<Vector2>();
        Queue<double> times = new Queue<double>();

        Vector2 linePoint, linePointDragStart;
        bool dragStarted;
        float radius = 50;

        double timer = 0;

        public void Render(FrameworkContext ctx) {
            timer += Time.DeltaTime;

            points.Enqueue(linePoint);
            times.Enqueue(timer);

            // remove points after 0.5 seconds
            if (timer - times.Peek() > 0.5f) {
                points.Dequeue();
                times.Dequeue();
            }

            if (Intersections.IsInsideCircle(ctx.MouseX, ctx.MouseY, linePoint.X, linePoint.Y, radius)) {
                ctx.SetDrawColor(1, 0, 0, 0.5f);
                if (ctx.MouseButtonIsDown(MouseButton.Any)) {
                    linePointDragStart = linePoint;
                    dragStarted = true;
                }
            } else {
                ctx.SetDrawColor(0, 0, 1, 0.5f);
            }

            // TODO: || ctx.window.LostFocus
            if (dragStarted && !ctx.MouseButtonIsDown(MouseButton.Any)) {
                dragStarted = false;
            }

            if (dragStarted) {
                linePoint = new Vector2(
                    MathHelper.Clamp(ctx.MouseX, ctx.VW * 0.25f, ctx.VW * 0.75f),
                    MathHelper.Clamp(ctx.MouseY, ctx.VH * 0.25f, ctx.VH * 0.75f)
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
