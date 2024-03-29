﻿using MinimalAF;
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

        DrawableFont _font = new DrawableFont("", new DrawableFontOptions { });

        public void Render(AFContext ctx) {
            timer += Time.DeltaTime;

            points.Enqueue(linePoint);
            times.Enqueue(timer);

            if (ctx.KeyJustPressed(KeyCode.R)) {
                linePoint = new Vector2 {
                    X = ctx.VW * 0.5f, Y = ctx.VH * 0.5f
                };
            }

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
            var instructions = "Mouse test (And polyline test) - Drag that point with your mouse";
            _font.DrawText(ctx, instructions, 16, new DrawTextOptions { 
                X = 0, Y = ctx.VH, VAlign = 1.0f
            });

            IM.DrawRectOutline(ctx, 5, new Rect(ctx.VW * 0.25f, ctx.VH * ctx.VH * 0.25f, ctx.VW * 0.75f, ctx.VH * 0.75f));

            if (points.Count < 2)
                return;

            var nLineDrawer = IM.NewNLineDrawer(ctx);
            var i = 0;
            foreach (Vector2 p in points) {
                nLineDrawer.ExtendNLine(
                    new Vertex { PosX = p.X, PosY = p.Y },
                    new Vertex { PosX = p.X, PosY = p.Y + 10 }
                );
                i++;
            }
            IM.DrawCircle(ctx, linePoint.X, linePoint.Y, radius);
        }
    }
}
