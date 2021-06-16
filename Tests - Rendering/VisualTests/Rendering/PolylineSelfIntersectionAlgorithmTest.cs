using MinimalAF.Logic;
using MinimalAF.Rendering;
using MinimalAF.Util;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
    public class PolylineSelfIntersectionAlgorithmTest : EntryPoint
    {
        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "PolylineSelfIntersectionAlgorithmTest";
            CTX.SetClearColor(1, 1, 1, 1);
            CTX.SetCurrentFont("Segoe UI", 24);
        }

        public override void Render(double deltaTime)
        {
            CTX.SetDrawColor(0, 0, 0, 1f);
            for (int i = 0; i < _points.Length; i++)
            {
                CTX.DrawCircle(_points[i].X, _points[i].Y, 10);
                CTX.DrawText(i.ToString(), _points[i].X + 20, _points[i].Y + 20);
            }

            CTX.SetDrawColor(0, 0, 1, 0.5f);

            PointF p1 = _points[0];
            PointF p2 = _points[1];
            PointF p3 = _points[2];
            PointF p4 = _points[3];

            float thickness = 100;

            CTX.BeginPolyLine(p1.X, p1.Y, thickness, CapType.None);
            CTX.AppendToPolyLine(p2.X, p2.Y);
            CTX.AppendToPolyLine(p3.X, p3.Y);
            CTX.EndPolyLine(p4.X, p4.Y);

            CTX.SetDrawColor(1, 0, 0, 1);
            DrawPerpendicular(p1, p2, thickness/2);
            DrawPerpendicular(p1, p2, p3, thickness/2);
            DrawPerpendicular(p2, p3, p4, thickness / 2);

            DrawAlgorithmDebug();
        }

        private static void DrawPerpendicular(PointF p1, PointF p2, float thickness)
        {
            PointF perp = CalculatePerpVector(p1, p2, thickness);

            CTX.DrawLine(p1.X, p1.Y, p1.X + perp.X, p1.Y + perp.Y, 2, CapType.None);
            CTX.DrawLine(p1.X, p1.Y, p1.X - perp.X, p1.Y - perp.Y, 2, CapType.None);
        }

        private static PointF CalculatePerpVector(PointF p1, PointF p2, float thickness)
        {
            PointF dir = MathUtilPF.Sub(p2, p1);
            float mag = MathUtilPF.Mag(dir);

            PointF perp = MathUtilPF.Times(dir, thickness / mag);
            perp = new PointF(-perp.Y, perp.X);
            return perp;
        }

        private static void DrawPerpendicular(PointF p1, PointF p2, PointF p3, float thickness)
        {
            PointF perp1 = CalculatePerpVector(p1, p2, thickness);
            PointF perp2 = CalculatePerpVector(p2, p3, thickness);
            PointF perp = MathUtilPF.Times(MathUtilPF.Add(perp1, perp2), 0.5f);

            float mag = MathUtilPF.Mag(perp);
            perp = MathUtilPF.Times(perp, thickness / mag);

            CTX.DrawLine(p2.X, p2.Y, p2.X + perp.X, p2.Y + perp.Y, 2, CapType.None);
            CTX.DrawLine(p2.X, p2.Y, p2.X - perp.X, p2.Y - perp.Y, 2, CapType.None);
        }

        private void DrawAlgorithmDebug()
        {
        }

        int _currentPoint = 0;

        PointF[] _points = new PointF[4];

        public override void Update(double deltaTime)
        {
            if (Input.IsMouseClicked(MouseButton.Left))
            {
                _points[_currentPoint] = new PointF(Input.MouseX, Input.MouseY);
                _currentPoint = (_currentPoint + 1) % _points.Length;
            }
        }
    }
}
