using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	public class StencilTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Stencil rendering test";

           SetClearColor(Color4.RGBA(0,0,0,0));
            SetFont("Consolas", 24);

			this.SetChildren(geometryAndTextTest);

			base.OnStart();

        }

        GeometryAndTextTest geometryAndTextTest = new GeometryAndTextTest();

        float _xPos = 0;

        public override void OnRender()
        {
			SetDrawColor(1, 1, 1, 1);
			Text("Stencil test", 0, Height, HorizontalAlignment.Left, VerticalAlignment.Top);

			StartStencillingWithoutDrawing(true);

            float barSize = MathF.Abs((Height / 2 - 5) * MathF.Sin(_time / 4f));
            Rect(0, Height, Width, Height - barSize);
            Rect(0, 0, Width, barSize);

            StartUsingStencil();

			base.OnRender();

            LiftStencil();

            StartStencillingWhileDrawing();

            float size = 60;
            DrawRedRectangle(size, _xPos);

            StartUsingStencil();

            size = 70;
            DrawBlueRectangle(size, _xPos);

            LiftStencil();
        }

        private void DrawBlueRectangle(float size, float xPos)
        {
            SetTexture(null);
            SetDrawColor(0, 0, 1, 1);
            Rect(Width / 2 - size + xPos, Height / 2 - size,
                Width / 2 + size + xPos, Height / 2 + size);
        }

        private void DrawRedRectangle(float size, float xPos)
        {
            SetTexture(null);
            SetDrawColor(1, 0, 0, 1);
            Rect(Width / 2 - size + xPos, Height / 2 - size,
                Width / 2 + size + xPos, Height / 2 + size);
        }

        float _time = 0;

        public override void OnUpdate()
        {
            _time += (float)Time.DeltaTime;

			base.OnUpdate();
			_xPos = 200 * MathF.Sin(_time / 2.0f);
        }
    }
}
