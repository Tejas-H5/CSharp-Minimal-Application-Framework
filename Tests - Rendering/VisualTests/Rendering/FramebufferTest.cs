using MinimalAF.Rendering;

namespace MinimalAF.VisualTests.Rendering {
    [VisualTest(
    description: @"Tests that framebuffers can be used properly.
The red square must be fully visible under the circles.
The part where the circles overlap must not be visible.
There must be a small orange rectangle in the middle
It must all be inside the green square
This text must be 0,0,0 black 
The circles must actually be circular, and not a distorted oval",
    tags: "2D, Framebuffer"
)]
    public class FramebufferTest : Element {
        Framebuffer fb;
        Texture monkeyTex;
        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "FramebufferTest";

            //w.RenderFrequency = 120;
            //w.UpdateFrequency = 120; 20;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));

            fb = new Framebuffer(1, 1);
            fb.Resize(800, 600);

            Clipping = false;

            monkeyTex = TextureMap.Load("monke", "./Res/monke texture.png");
        }


        double timer = 0;

        public override void OnUpdate() {
            timer += Time.DeltaTime;
        }

        public override void OnRender() {
            var prevTransform = CTX.Shader.Model;
            Rect prevScreenRect = screenRect;

            screenRect = new Rect(0, 0, 800, 600);

            RedirectDrawCalls(fb);
            Clear(RGB(0, 0, 0, 0));

            SetViewProjectionCartesian2D(1, 1, 0, 0);
            SetTransform(Translation(0, 0, 0));
            
            float wCX = 400;
            float wCY = 300;
            SetDrawColor(0, 0, 1, 1);
            DrawDualCirclesCenter(wCX, wCY);
            //Rect(0, 0, 800, 600);

            SetDrawColor(1, 1, 0, 1);
            Rect(wCX, wCY, wCX + 50, wCY + 25);


            RedirectDrawCalls(null);
            screenRect = prevScreenRect;
            ResetCoordinates();

            SetDrawColor(1, 0, 0, 1);
            float rectSize = 200;
            Rect(wCX - rectSize, wCY - rectSize, wCX + rectSize, wCY + rectSize);

            SetTexture(fb.Texture);
            SetDrawColor(1, 1, 1, 0.5f);
            Rect(0, 0, 800, 600);


            SetTexture(null);

            SetDrawColor(0, 1, 0, 0.5f);
            RectOutline(10, wCX - wCY, wCY - wCY, wCX + wCY, wCY + wCY);
        }

        private void DrawDualCirclesCenter(float x, float y) {
            Circle(x - 100, y - 100, 200);
            Circle(x + 100, y + 100, 200);
        }
    }
}
