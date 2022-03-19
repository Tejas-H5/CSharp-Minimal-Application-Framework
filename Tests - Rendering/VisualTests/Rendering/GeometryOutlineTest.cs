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

            ClearColor = Color4.RGBA(1, 1, 1, 1);
        }


        public override void OnUpdate()
        {
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 0, 0, 0.5f);

            CTX.Rect.Draw(20, 20, 100, 100);
            CTX.Circle.Draw(500, 500, 200);
            CTX.Arc.Draw(200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            CTX.Arc.Draw(300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            CTX.Arc.Draw(400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            CTX.Line.Draw(Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            CTX.Line.Draw(Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            CTX.Line.Draw(lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);


            CTX.SetDrawColor(0, 0, 1, 1f);

            CTX.Rect.DrawOutline(5, 20, 20, 100, 100);
            CTX.Circle.DrawOutline(10, 500, 500, 200);
            CTX.Arc.DrawOutline(10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            CTX.Arc.DrawOutline(10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            CTX.Arc.DrawOutline(10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            CTX.Line.DrawOutline(10, Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            CTX.Line.DrawOutline(10, Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);

            CTX.Line.DrawOutline(10, lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);
        }
    }
}
