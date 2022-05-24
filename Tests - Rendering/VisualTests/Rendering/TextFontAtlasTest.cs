using MinimalAF.Rendering;
using MinimalAF;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Tests that font loading is working. It is failing at the moment.",
        tags: "2D, text"
    )]
    class TextFontAtlasText : Element {
        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "text font atlas test";
            //w.RenderFrequency = 120; 60;
            //w.UpdateFrequency = 120; 120;

            SetClearColor(Color.White);
        }

        float pos = 0;
        public override void OnUpdate() {
            pos += 50 * MousewheelNotches;
        }


        public override void OnRender() {
            SetDrawColor(0, 0, 0, 1);
            SetFont("Consolas", 16);

            var tex = CTX.InternalFontTexture;
            SetTexture(tex);

            var cX = VW(0.5f);
            var cY = VH(0.5f);
            var rect = new Rect(
                cX - tex.Width / 2f, cY - tex.Height / 2f,
                cX + tex.Width / 2f, cY + tex.Height / 2f
            );

            DrawRect(rect);
        }
    }
}
