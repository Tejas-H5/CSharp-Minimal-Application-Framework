using MinimalAF.Logic;
using MinimalAF.Rendering;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
    public class PolylineTest : EntryPoint
    {
        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Polyline";

            Window.RenderFrequency = 120;
            Window.UpdateFrequency = 120;

            CTX.SetClearColor(1, 1, 1, 1);
        }

        Queue<PointF> _points = new Queue<PointF>();
        Queue<double> _times = new Queue<double>();


        double timer = 0;

        public override void Update(double deltaTime)
        {
            timer += deltaTime;

            _points.Enqueue(new PointF(Input.MouseX, Input.MouseY));
            _times.Enqueue(timer);
            
            if (timer - _times.Peek() > 0.5f)
            {
                _points.Dequeue();
                _times.Dequeue();
            }
        }

        public override void Render(double deltaTime)
        {
            if (_points.Count < 2)
                return;

            CTX.SetDrawColor(0, 0, 1, 0.5f);

            int i = 0;
            foreach (PointF p in _points)
            {
                if (i == 0)
                {
                    CTX.BeginPolyLine(p.X, p.Y, 50, CapType.Circle);
                }
                else if(i == _points.Count - 1)
                {
                    CTX.EndPolyLine(p.X, p.Y);
                }
                else
                {
                    CTX.AppendToPolyLine(p.X, p.Y);
                }

                i++;
            }
        }
    }
}
