using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	[VisualTest(
        description: @"Test that the stencilling functionality is working.
The red square must appear above the blue square, and there should be vertical bars that retract and extend, masking
the visibility of another test.",
        tags: "2D, stencil"
    )]
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
			DrawText("Stencil test", 0, Height, HAlign.Left, VAlign.Top);

			StartStencillingWithoutDrawing(true);

            float barSize = MathF.Abs((Height / 2 - 5) * MathF.Sin(time / 4f));
            DrawRect(0, Height - barSize, Width, Height);
            DrawRect(0, 0, Width, barSize);

            StartUsingStencil();
        }

        public override void AfterRender() {
            // TODO low priority: make stencilling stack based. Any of the children elements could have called this and made wierd stuff happen
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
            DrawRect(Width / 2 - size + xPos, Height / 2 - size,
                Width / 2 + size + xPos, Height / 2 + size);
        }

        private void DrawRedRectangle(float size, float xPos)
        {
            SetTexture(null);
            SetDrawColor(1, 0, 0, 1);
            DrawRect(Width / 2 - size + xPos, Height / 2 - size,
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
