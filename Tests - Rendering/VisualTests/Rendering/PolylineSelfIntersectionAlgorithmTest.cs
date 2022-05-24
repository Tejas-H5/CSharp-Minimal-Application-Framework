using MinimalAF;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RenderingEngineVisualTests
{
	[VisualTest(
        description: @"Testing an algorithm that the polyline function uses to detect self-intersecting triangles.
Intersections should be clearly marked with red lines.",
        tags: "2D, polyline"
    )]
	public class PolylineSelfIntersectionAlgorithmTest : Element
    {
        public override void OnMount(Window w)
        {
            
            w.Size = (800, 600);
            w.Title = "PolylineSelfIntersectionAlgorithmTest";
            SetClearColor(Color.RGBA(1, 1, 1, 1));
            SetFont("Segoe UI", 24);
        }

        public override void OnRender()
        {
            SetDrawColor(0, 0, 0, 1f);
            for (int i = 0; i < points.Length; i++)
            {
                DrawCircle(points[i].X, points[i].Y, 10);
                DrawText(i.ToString(), points[i].X + 20, points[i].Y + 20);
            }

			int fbbinding = GL.GetInteger(GetPName.DrawFramebufferBinding);
			int readfbbinding = GL.GetInteger(GetPName.ReadFramebufferBinding);

			SetDrawColor(0, 0, 1, 0.5f);

            var p1 = points[0];
            var p2 = points[1];
            var p3 = points[2];
            var p4 = points[3];

            float radius = 100;

            StartPolyLine(p1.X, p1.Y, radius, CapType.None);
            ContinuePolyLine(p2.X, p2.Y);
            ContinuePolyLine(p3.X, p3.Y);
            EndPolyLine(p4.X, p4.Y);


            Vector2 lastPerp;

            lastPerp = DrawFirstPerpendicular(p1, p2, radius);
            lastPerp = DrawAndDebugPerpendicular(p1, p2, p3, lastPerp, radius);
            lastPerp = DrawAndDebugPerpendicular(p2, p3, p4, lastPerp, radius);

            DrawAlgorithmDebug();
        }

        private Vector2 DrawFirstPerpendicular(Vector2 p1, Vector2 p2, float thickness)
        {
            Vector2 perp = CalculatePerpVector(p1, p2, thickness);

            SetDrawColor(0, 1, 0, 1);

            DrawLine(p1.X, p1.Y, p1.X + perp.X, p1.Y + perp.Y, 2, CapType.None);
            DrawLine(p1.X, p1.Y, p1.X - perp.X, p1.Y - perp.Y, 2, CapType.None);

            return perp;
        }

        private static Vector2 CalculatePerpVector(Vector2 p1, Vector2 p2, float thickness)
        {
            Vector2 dir = p2- p1;
            float mag = dir.Length;

            Vector2 perp = dir * (thickness / mag);
            perp = new Vector2(-perp.Y, perp.X);
            return perp;
        }

        private Vector2 DrawAndDebugPerpendicular(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 lastPerp, float thickness)
        {
            var perp1 = CalculatePerpVector(p1, p2, thickness);
            var perp2 = CalculatePerpVector(p2, p3, thickness);
            var perp = 0.5f * perp1 + 0.5f * perp2;

            float mag = perp.Length;
            perp = perp * (thickness / mag);


            var dir = Vec2(lastPerp.Y, -lastPerp.X);
            var vec1 = p2 + perp - p1;
            var vec2 = p2 + perp - p1;

            float dotp1 = Vector2.Dot(vec1, dir);
            float dotp2 = Vector2.Dot(vec2, dir);

            bool isSelfIntersecting = (dotp1 < 0 || dotp2 < 0);

            if (!isSelfIntersecting)
                SetDrawColor(0, 1, 0, 1);
            else
                SetDrawColor(1, 0, 0, 1);

            DrawLine(p2.X, p2.Y, p2.X + perp.X, p2.Y + perp.Y, 2, CapType.None);
            DrawLine(p2.X, p2.Y, p2.X - perp.X, p2.Y - perp.Y, 2, CapType.None);

            SetDrawColor(Color.VA(0, 0.5f));
            DrawLine(p1.X, p1.Y, p1.X + vec1.X, p1.Y + vec1.Y, 2, CapType.None);
            DrawLine(p1.X, p1.Y, p1.X + vec2.X, p1.Y + vec2.Y, 2, CapType.None);

            return perp;
        }

        private void DrawAlgorithmDebug()
        {
        }

        int currentPoint = 0;

        Vector2[] points = new Vector2[4];

        public override void OnUpdate()
        {
            if (MouseButtonPressed(MouseButton.Left))
            {
                points[currentPoint] = new Vector2(MouseX, MouseY);
                currentPoint = (currentPoint + 1) % points.Length;
            }
        }
    }
}
