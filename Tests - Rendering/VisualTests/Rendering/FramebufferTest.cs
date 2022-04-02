using MinimalAF.Rendering;

namespace MinimalAF.VisualTests.Rendering
{
	public class FramebufferTest : Element
    {
        public override void OnMount(Window w)
        {
            
            w.Size = (800, 600);
            w.Title = "FramebufferTest";

            //w.RenderFrequency = 120;
            //w.UpdateFrequency = 120; 20;

           SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }


        double timer = 0;

        public override void OnUpdate()
        {
            timer += Time.DeltaTime;
        }

        public override void OnRender()
        {
            UseTransparentFramebuffer(0);

            SetDrawColor(0, 0, 1, 1);

            float wCX = VW(0.5f);
            float wCY = VH(0.5f);
            DrawDualCirclesCenter(wCX, wCY);
            SetDrawColor(1, 1, 0, 1);
            Rect(wCX, wCY, wCX + 50, wCY + 25);

            StopUsingFramebuffer();

            SetDrawColor(Color4.RGBA(0, 0, 0, 1));
            SetFont("Consolas", 12);
            Text("The red square must be fully visible under the circles.\n" +
                "The part where the circles overlap must not be visible.\n" +
                "There must be a small orange rectangle in the middle\n" +
				"It must all be inside the green square\n" +
				"This text must be 0,0,0 black \n",
                0, Height - 20);

            SetDrawColor(1, 0, 0, 1);

            float rectSize = 200;


            Rect(wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);
            SetDrawColor(1, 1, 1, 0.5f);
            SetTexture(GetFramebufferTexture(0));
            
            Rect(0, 0, Width,Height);

            SetTexture(null);

            SetDrawColor(0, 1, 0, 0.5f);
            RectOutline(10, wCX - 300, wCY - 300, wCX + 300, wCY + 300);
        }


        private void DrawDualCirclesCenter(float x, float y)
        {
            Circle(x - 100, y - 100, 200);
            Circle(x + 100, y + 100, 200);
        }
    }
}
