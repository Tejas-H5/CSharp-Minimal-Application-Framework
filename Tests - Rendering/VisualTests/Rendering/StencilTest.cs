using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	[VisualTest]
	public class StencilTest : Element
    {
        public override void OnMount(Window w)
        {
            
            w.Size = (800, 600);
            w.Title = "Stencil rendering test";

            SetClearColor(Color4.White);
            SetFont("Consolas", 24);

			SetChildren(geometryAndTextTest);

			base.OnMount(w);

        }

        GeometryAndTextTest geometryAndTextTest = new GeometryAndTextTest();

        float xPos = 0;

        public override void OnRender()
        {
			SetDrawColor(1, 1, 1, 1);
			Text("Stencil test", 0, Height, HorizontalAlignment.Left, VerticalAlignment.Top);

			StartStencillingWithoutDrawing(true);

            float barSize = MathF.Abs((Height / 2 - 5) * MathF.Sin(time / 4f));
            Rect(0, Height, Width, Height - barSize);
            Rect(0, 0, Width, barSize);

            StartUsingStencil();
        }

        public override void AfterRender() {
            // TODO low priority: make this stack based. Any of the children elements could have called this and made wierd stuff happen
            LiftStencil();

            StartStencillingWhileDrawing();

            float size = 60;
            DrawRedRectangle(size, xPos);

            StartUsingStencil();

            size = 70;
            DrawBlueRectangle(size, xPos);

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

        float time = 0;

        public override void OnUpdate()
        {
            time += (float)Time.DeltaTime;
			xPos = 200 * MathF.Sin(time / 2.0f);
        }

        public override void OnLayout() {
            geometryAndTextTest.RelativeRect = new Rect(0, 0, VW(1), VH(1));

            LayoutChildren();
        }

    }
}
