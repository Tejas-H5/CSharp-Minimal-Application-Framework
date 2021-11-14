using MinimalAF;
using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
    class GeometryOutlineTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Triangle";

            CTX.SetClearColor(1, 1, 1, 1);
        }


        public override void OnUpdate()
        {
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 0, 0, 0.5f);

            CTX.DrawRect(20, 20, 100, 100);
            CTX.DrawCircle(500, 500, 200);
            CTX.DrawArc(200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            CTX.DrawArc(300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            CTX.DrawArc(400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            CTX.DrawLine(Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            CTX.DrawLine(Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            CTX.DrawLine(lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);


            CTX.SetDrawColor(0, 0, 1, 1f);

            CTX.DrawRectOutline(5, 20, 20, 100, 100);
            CTX.DrawCircleOutline(10, 500, 500, 200);
            CTX.DrawArcOutline(10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            CTX.DrawArcOutline(10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            CTX.DrawArcOutline(10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            CTX.DrawLineOutline(10, Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            CTX.DrawLineOutline(10, Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);

            CTX.DrawLineOutline(10, lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);
        }
    }
}
