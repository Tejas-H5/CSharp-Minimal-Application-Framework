using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF.Util;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
    public class PolylineSelfIntersectionAlgorithmTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "PolylineSelfIntersectionAlgorithmTest";
            CTX.SetClearColor(1, 1, 1, 1);
            CTX.Text.SetFont("Segoe UI", 24);
        }

        public override void OnRender()
        {
            CTX.SetDrawColor(0, 0, 0, 1f);
            for (int i = 0; i < _points.Length; i++)
            {
                CTX.Circle.Draw(_points[i].X, _points[i].Y, 10);
                CTX.Text.Draw(i.ToString(), _points[i].X + 20, _points[i].Y + 20);
            }

            CTX.SetDrawColor(0, 0, 1, 0.5f);

            PointF p1 = _points[0];
            PointF p2 = _points[1];
            PointF p3 = _points[2];
            PointF p4 = _points[3];

            float thickness = 200;

            CTX.NLine.Begin(p1.X, p1.Y, thickness, CapType.None);
            CTX.NLine.Continue(p2.X, p2.Y);
            CTX.NLine.Continue(p3.X, p3.Y);
            CTX.NLine.End(p4.X, p4.Y);


            PointF lastPerp;

            lastPerp = DrawFirstPerpendicular(p1, p2, thickness/2);
            lastPerp = DrawAndDebugPerpendicular(p1, p2, p3, lastPerp, thickness / 2);
            lastPerp = DrawAndDebugPerpendicular(p2, p3, p4, lastPerp, thickness / 2);

            DrawAlgorithmDebug();
        }

        private static PointF DrawFirstPerpendicular(PointF p1, PointF p2, float thickness)
        {
            PointF perp = CalculatePerpVector(p1, p2, thickness);

            CTX.SetDrawColor(0, 1, 0, 1);

            CTX.Line.Draw(p1.X, p1.Y, p1.X + perp.X, p1.Y + perp.Y, 2, CapType.None);
            CTX.Line.Draw(p1.X, p1.Y, p1.X - perp.X, p1.Y - perp.Y, 2, CapType.None);

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

        private static PointF DrawAndDebugPerpendicular(PointF p1, PointF p2, PointF p3, PointF lastPerp, float thickness)
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
                CTX.SetDrawColor(0, 1, 0, 1);
            else
                CTX.SetDrawColor(1, 0, 0, 1);

            CTX.Line.Draw(p2.X, p2.Y, p2.X + perp.X, p2.Y + perp.Y, 2, CapType.None);
            CTX.Line.Draw(p2.X, p2.Y, p2.X - perp.X, p2.Y - perp.Y, 2, CapType.None);

            CTX.SetDrawColor(Color4.VA(0, 0.5f));
            CTX.Line.Draw(p1.X, p1.Y, p1.X + vec1.X, p1.Y + vec1.Y, 2, CapType.None);
            CTX.Line.Draw(p1.X, p1.Y, p1.X + vec2.X, p1.Y + vec2.Y, 2, CapType.None);

            return perp;
        }

        private void DrawAlgorithmDebug()
        {
        }

        int _currentPoint = 0;

        PointF[] _points = new PointF[4];

        public override void OnUpdate()
        {
            if (Input.Mouse.IsPressed(MouseButton.Left))
            {
                _points[_currentPoint] = new PointF(Input.Mouse.X, Input.Mouse.Y);
                _currentPoint = (_currentPoint + 1) % _points.Length;
            }
        }
    }
}
