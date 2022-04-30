using MinimalAF.Rendering;
using MinimalAF.Util;
using System.Drawing;
using OpenTK.Graphics.OpenGL;


namespace MinimalAF.VisualTests.Rendering
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
            SetClearColor(Color4.RGBA(1, 1, 1, 1));
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

            PointF p1 = points[0];
            PointF p2 = points[1];
            PointF p3 = points[2];
            PointF p4 = points[3];

            float radius = 100;

            StartPolyLine(p1.X, p1.Y, radius, CapType.None);
            ContinuePolyLine(p2.X, p2.Y);
            ContinuePolyLine(p3.X, p3.Y);
            EndPolyLine(p4.X, p4.Y);


            PointF lastPerp;

            lastPerp = DrawFirstPerpendicular(p1, p2, radius);
            lastPerp = DrawAndDebugPerpendicular(p1, p2, p3, lastPerp, radius);
            lastPerp = DrawAndDebugPerpendicular(p2, p3, p4, lastPerp, radius);

            DrawAlgorithmDebug();
        }

        private PointF DrawFirstPerpendicular(PointF p1, PointF p2, float thickness)
        {
            PointF perp = CalculatePerpVector(p1, p2, thickness);

            SetDrawColor(0, 1, 0, 1);

            DrawLine(p1.X, p1.Y, p1.X + perp.X, p1.Y + perp.Y, 2, CapType.None);
            DrawLine(p1.X, p1.Y, p1.X - perp.X, p1.Y - perp.Y, 2, CapType.None);

            return perp;
        }

        private static PointF CalculatePerpVector(PointF p1, PointF p2, float thickness)
        {
            PointF dir = MathUtilPF.Sub(p2, p1);
            float mag = MathUtilPF.Mag(dir);

            PointF perp = MathUtilPF.Times(dir, thickness / mag);
            perp = new PointF(-perp.Y, perp.X);
            return perp;
        }

        private PointF DrawAndDebugPerpendicular(PointF p1, PointF p2, PointF p3, PointF lastPerp, float thickness)
        {
            PointF perp1 = CalculatePerpVector(p1, p2, thickness);
            PointF perp2 = CalculatePerpVector(p2, p3, thickness);
            PointF perp = MathUtilPF.Times(MathUtilPF.Add(perp1, perp2), 0.5f);

            float mag = MathUtilPF.Mag(perp);
            perp = MathUtilPF.Times(perp, thickness / mag);


            PointF dir = new PointF(lastPerp.Y, -lastPerp.X);
            PointF vec1 = MathUtilPF.Sub(MathUtilPF.Add(p2, perp), p1);
            PointF vec2 = MathUtilPF.Sub(MathUtilPF.Sub(p2, perp), p1);

            float dotp1 = MathUtilPF.Dot(vec1, dir);
            float dotp2 = MathUtilPF.Dot(vec2, dir);

            bool isSelfIntersecting = (dotp1 < 0 || dotp2 < 0);

            if (!isSelfIntersecting)
                SetDrawColor(0, 1, 0, 1);
            else
                SetDrawColor(1, 0, 0, 1);

            DrawLine(p2.X, p2.Y, p2.X + perp.X, p2.Y + perp.Y, 2, CapType.None);
            DrawLine(p2.X, p2.Y, p2.X - perp.X, p2.Y - perp.Y, 2, CapType.None);

            SetDrawColor(Color4.VA(0, 0.5f));
            DrawLine(p1.X, p1.Y, p1.X + vec1.X, p1.Y + vec1.Y, 2, CapType.None);
            DrawLine(p1.X, p1.Y, p1.X + vec2.X, p1.Y + vec2.Y, 2, CapType.None);

            return perp;
        }

        private void DrawAlgorithmDebug()
        {
        }

        int currentPoint = 0;

        PointF[] points = new PointF[4];

        public override void OnUpdate()
        {
            if (MouseButtonPressed(MouseButton.Left))
            {
                points[currentPoint] = new PointF(MouseX, MouseY);
                currentPoint = (currentPoint + 1) % points.Length;
            }
        }
    }
}
