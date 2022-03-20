using MinimalAF.Rendering;

namespace MinimalAF.VisualTests.Rendering
{
	class TextFontAtlasText : Element
    {
        public override void OnMount()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "text font atlas test";
            //w.RenderFrequency = 120; 60;
            //w.UpdateFrequency = 120; 120;

			SetClearColor(Color4.RGBA(0, 0, 0, 0));

			SetFont("Consolas", 24);
        }


        double timer = 0;
        public override void OnUpdate()
        {
            //*
            timer += Time.DeltaTime;
            if (timer < 0.05)
                return;
            //*/
            timer = 0;

            if (MouseWheelNotches != 0)
            {
                pos += 50 * MouseWheelNotches;
            }
        }

        float pos = 0;

        public override void OnRender()
        {
            //Draw font atlas offset by vertical scroll in pos variable
        }
    }
}
