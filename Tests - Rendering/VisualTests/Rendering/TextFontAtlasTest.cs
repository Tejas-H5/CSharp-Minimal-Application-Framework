using MinimalAF.Rendering;

namespace MinimalAF.VisualTests.Rendering
{
    [VisualTest(
        description: @"Tests that font loading is working. It is failing at the moment.",
        tags: "2D, text"
    )]
	class TextFontAtlasText : Element
    {
        public override void OnMount(Window w)
        {
            
            w.Size = (800, 600);
            w.Title = "text font atlas test";
            //w.RenderFrequency = 120; 60;
            //w.UpdateFrequency = 120; 120;

            SetClearColor(Color4.White);
        }

        float pos = 0;
        public override void OnUpdate()
        {
             pos += 50 * MousewheelNotches;
        }


        public override void OnRender()
        {
            //Draw font atlas offset by vertical scroll in pos variable
            SetDrawColor(0, 0, 0, 1);
            SetFont("Consolas", 24);
        }
    }
}
