using MinimalAF;
using MinimalAF.Rendering;

namespace MinimalAF.VisualTests.Rendering
{
    class TextFontAtlasText : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "text font atlas test";
            //w.RenderFrequency = 120; 60;
            //w.UpdateFrequency = 120; 120;

            CTX.SetClearColor(0, 0, 0, 0);

            CTX.Text.SetFont("Consolas", 24);
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

            if (Input.Mouse.WheelNotches != 0)
            {
                pos += 50 * Input.Mouse.WheelNotches;
            }
        }

        float pos = 0;

        public override void OnRender()
        {
            //Draw font atlas offset by vertical scroll in pos variable
        }
    }
}
