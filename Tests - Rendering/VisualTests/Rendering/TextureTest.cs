using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	[VisualTest(
        description: @"Test that texture loading works. Also test that transforms work, cause why not 
(there are actually reasons why not, but I dont rlly care at the moment).
The texture on the right is with nearest neighbour, and the texture on the left is with bilinear, so it's a bit blurry on the edges).
Also, the texture on the left must be still, while only the one on the right is moving",
        tags: "2D, texture"
    )]
	class TextureTest : Element
    {
        Texture tex;
        Texture tex2;

        public override void OnMount(Window w)
        {
            
            w.Size = (800, 600);
            w.Title = "Texture loading test";
            //w.RenderFrequency = 120; 60;
            //w.UpdateFrequency = 120; 120;

           SetClearColor(Color4.White);

            TextureMap.Load("placeholder", "./Res/settings_icon.png", new TextureImportSettings());
            TextureMap.Load("placeholderNN", "./Res/settings_icon.png", new TextureImportSettings { Filtering = FilteringType.NearestNeighbour });

            tex = TextureMap.Get("placeholder");
            tex2 = TextureMap.Get("placeholderNN");
        }


        public override void OnUpdate()
        {
            t += (float)Time.DeltaTime;
        }

        float t = 0;

        public override void OnRender()
        {
            SetDrawColor(1, 1, 1, 1);

            SetTexture(tex);
            Rect(20, 20, Width / 2 - 20, Height - 20);

            Rect rect2 = new Rect(Width / 2 + 20, 20, Width - 20, Height - 20);


            SetTransform(
                Scale(MathF.Sin(t)) *
                Rotation(0, 0, t) *
                Translation(rect2.CenterX, rect2.CenterY, 0)
            );

			SetTexture(tex2);
			Rect(new Rect(-rect2.Width / 2, -rect2.Height / 2, rect2.Width / 2, rect2.Height / 2).Rectify());
        }
    }
}
