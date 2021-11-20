using MinimalAF.Rendering;
using System.Collections.Generic;
using System.Drawing;

namespace MinimalAF.VisualTests.Rendering
{
	public class PolylineTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Polyline";

            w.RenderFrequency = 120;
            //w.UpdateFrequency = 120; 20;

            CTX.SetClearColor(1, 1, 1, 1);
        }

        Queue<PointF> _points = new Queue<PointF>();
        Queue<double> _times = new Queue<double>();


        double timer = 0;

        public override void OnUpdate()
        {
            timer += Time.DeltaTime;

            _points.Enqueue(new PointF(Input.Mouse.X, Input.Mouse.Y));
            _times.Enqueue(timer);
            
            if (timer - _times.Peek() > 0.5f)
            {
                _points.Dequeue();
                _times.Dequeue();
            }
        }

        public override void OnRender()
        {
            if (_points.Count < 2)
                return;

            CTX.SetDrawColor(0, 0, 1, 0.5f);

            int i = 0;
            foreach (PointF p in _points)
            {
                if (i == 0)
                {
                    CTX.NLine.Begin(p.X, p.Y, 50, CapType.Circle);
                }
                else if(i == _points.Count - 1)
                {
                    CTX.NLine.End(p.X, p.Y);
                }
                else
                {
                    CTX.NLine.Continue(p.X, p.Y);
                }

                i++;
            }
        }
    }
}
