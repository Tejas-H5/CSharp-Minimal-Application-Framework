using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	[VisualTest(
        description: @"Test that the outline functionality works.",
        tags: "2D"
    )]
	class GeometryOutlineTest : Element
    {
        public override void OnMount(Window w)
        {
            w.Size = (800, 600);
            w.Title = "Triangle";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }


        public override void OnUpdate()
        {
        }

        public override void OnRender()
        {
            SetDrawColor(1, 0, 0, 0.5f);

            DrawRect(20, 20, 100, 100);
            DrawCircle(500, 500, 200);
            DrawArc(200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            DrawArc(300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            DrawArc(400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            DrawLine(Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            DrawLine(Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            DrawLine(lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);


            SetDrawColor(0, 0, 1, 1f);

            DrawRectOutline(5, 20, 20, 100, 100);
            DrawCircleOutline(10, 500, 500, 200);
            DrawArcOutline(10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            DrawArcOutline(10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            DrawArcOutline(10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            DrawLineOutline(10, Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            DrawLineOutline(10, Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);

            DrawLineOutline(10, lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);
        }
    }
}
