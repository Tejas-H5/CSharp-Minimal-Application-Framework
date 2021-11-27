using MinimalAF.Rendering;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	public class StencilTest : Element
    {
        public override void OnStart()
        {
            WindowElement w = GetAncestor<WindowElement>();
            w.Size = (800, 600);
            w.Title = "Stencil rendering test";

            CTX.SetClearColor(0, 0, 0, 1);
            CTX.Text.SetFont("Consolas", 24);

            geometryAndTextTest.Init();
        }

        GeometryAndTextTest geometryAndTextTest = new GeometryAndTextTest();

        float _xPos = 0;

        public override void OnRender()
        {
            CTX.StartStencillingWithoutDrawing(true);

            float barSize = MathF.Abs((Height / 2 - 5) * MathF.Sin(_time / 4f));
            CTX.Rect.Draw(0, Height, Width, Height - barSize);
            CTX.Rect.Draw(0, 0, Width, barSize);

            CTX.StartUsingStencil();

            geometryAndTextTest.OnRender();

            CTX.LiftStencil();

            CTX.StartStencillingWhileDrawing();

            float size = 60;
            DrawRedRectangle(size, _xPos);

            CTX.StartUsingStencil();

            size = 70;
            DrawBlueRectangle(size, _xPos);

            CTX.LiftStencil();
        }

        private void DrawBlueRectangle(float size, float xPos)
        {
            CTX.SetTexture(null);
            CTX.SetDrawColor(0, 0, 1, 1);
            CTX.Rect.Draw(Width / 2 - size + xPos, Height / 2 - size,
                Width / 2 + size + xPos, Height / 2 + size);
        }

        private void DrawRedRectangle(float size, float xPos)
        {
            CTX.SetTexture(null);
            CTX.SetDrawColor(1, 0, 0, 1);
            CTX.Rect.Draw(Width / 2 - size + xPos, Height / 2 - size,
                Width / 2 + size + xPos, Height / 2 + size);
        }

        float _time = 0;

        public override void OnUpdate()
        {
            _time += (float)Time.DeltaTime;

            geometryAndTextTest.OnUpdate();
            _xPos = 200 * MathF.Sin(_time / 2.0f);
        }

        public override void OnResize()
        {
            base.OnResize();

            geometryAndTextTest.RectTransform.Copy(RectTransform);
        }
    }
}
