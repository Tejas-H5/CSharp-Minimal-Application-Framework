using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngineVisualTests {
    [VisualTest(
        description: @"Tests that text rendering, geometry rendering and normal texture rendering can happen
all at once.",
        tags: "2D"
    )]
    class GeometryAndTextTest : Element {
        List<string> rain = new List<string>();
        Texture tex;
        TextTest textTest;

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Text and geometry test";

            SetClearColor(Color.White);

            SetFont("Consolas", 24);
            textTest = new TextTest();
            Init();

            SetChildren(textTest);

            // remove when we start using OnMount
            w.Title = "Text and geometry test";
        }

        public void Init() {
            TextureMap.Load("placeholder", "./Res/settings_icon.png", new TextureImportSettings {
                Filtering = FilteringType.NearestNeighbour
            });

            tex = TextureMap.Get("placeholder");
        }

        public override void OnUpdate() {
            a += (float)Time.DeltaTime;

            base.OnUpdate();
        }

        float a = 0;
        public override void AfterRender() {
            SetTexture(null);
            SetDrawColor(Color.RGBA(0, 1, 0, 0.5f));
            DrawArc(Width / 2, Height / 2, MathF.Min(Height / 2f, Width / 2f), a, MathF.PI * 2 + a, 6);

            SetTexture(tex);
            //RenderingContext.DrawFilledArc(Width / 2, Height / 2, MathF.Min(Height / 2f, Width / 2f)/2f, a/2f, MathF.PI * 2 + a/2f, 6);

            DrawRect(VW(0.5f) - 50, VH(0.5f) - 50, VW(0.5f) + 50, VH(0.5f) + 50);

            //RenderingContext.DrawRect(100,100,Width-100, Height-100);
        }

        public override void OnLayout() {
            textTest.RelativeRect = new Rect(0, 0, VW(1), VH(1));

            LayoutChildren();
        }
    }
}
