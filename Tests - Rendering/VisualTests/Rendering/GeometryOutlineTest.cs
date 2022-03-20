using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	class GeometryOutlineTest : Element
    {
        public override void OnMount()
        {
            Window w = GetAncestor<Window>();
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

            Rect(20, 20, 100, 100);
            Circle(500, 500, 200);
            Arc(200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            Arc(300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            Arc(400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            Line(Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            Line(Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);


            int lineSize = 100;
            Line(lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);


            SetDrawColor(0, 0, 1, 1f);

            RectOutline(5, 20, 20, 100, 100);
            CircleOutline(10, 500, 500, 200);
            ArcOutline(10, 200, 200, 50, MathF.PI / 2, 3 * MathF.PI / 2, 3);
            ArcOutline(10, 300, 300, 50, MathF.PI / 2, 3 * MathF.PI / 2, 2);
            ArcOutline(10, 400, 400, 50, MathF.PI / 2, 3 * MathF.PI / 2, 1);

            LineOutline(10, Width - 60, 600, Width - 100, 200, 10.0f, CapType.None);
            LineOutline(10, Width - 100, 600, Width - 130, 200, 10.0f, CapType.Circle);

            LineOutline(10, lineSize, lineSize, Width - lineSize, Height - lineSize, lineSize / 2, CapType.Circle);
        }
    }
}
