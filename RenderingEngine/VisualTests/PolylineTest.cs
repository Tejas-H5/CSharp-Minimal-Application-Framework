using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RenderingEngine.VisualTests
{
    public class PolylineTest : EntryPoint
    {
        public override void Start()
        {

            Window.Size = (800, 600);
            Window.Title = "Polyline";
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);
        }


        List<PointF> _points = new List<PointF>();

        public override void Update(double deltaTime)
        {
            if (Input.MouseClicked(MouseButton.Left))
            {
                _points.Add(new PointF(Input.MouseX, Input.MouseY));
            }
        }

        public override void Render(double deltaTime)
        {
            CTX.Clear();
            CTX.SetDrawColor(0, 0, 1, 0.5f);

            if(_points.Count > 0)
            {
                CTX.BeginPolyLine(_points[0].X, _points[0].Y, 10, Rendering.ImmediateMode.CapType.None);

                for (int i = 1; i < _points.Count - 1; i++)
                {
                    CTX.AppendToPolyLine(_points[i].X, _points[i].Y);
                }

                CTX.EndPolyLine(_points[_points.Count - 1].X, _points[_points.Count - 1].Y);
            }


            CTX.SwapBuffers();
        }
    }
}
