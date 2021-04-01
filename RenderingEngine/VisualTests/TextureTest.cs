using OpenTK.Mathematics;
using RenderingEngine.Rendering;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using RenderingEngine.Datatypes;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class TextureTest : EntryPoint
    {
        Texture _tex;
        Texture _tex2;

        public override void Start(RenderingContext ctx, GraphicsWindow window)
        {
			base.Start(ctx, window);
            window.Size=(800, 600);
            window.Title=("Texture loading test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            ctx.SetClearColor(0, 0, 0, 1);

            TextureMap.RegisterTexture("placeholder", "settings_icon.png", new TextureImportSettings());
            TextureMap.RegisterTexture("placeholderNN", "settings_icon.png", new TextureImportSettings { Filtering = FilteringType.NearestNeighbour } );

            _tex = TextureMap.GetTexture("placeholder");
            _tex2 = TextureMap.GetTexture("placeholderNN");
        }


        public override void Update(double deltaTime)
        {

        }


        public override void Render(double deltaTime)
        {
            ctx.Clear();

            ctx.SetDrawColor(1,1,1,1);

            ctx.SetTexture(_tex);
            ctx.DrawRect(20, 20, window.Width/2 - 20, window.Height - 20);

            ctx.SetTexture(_tex2);
            ctx.DrawRect(new Rect2D(window.Width / 2 + 20, 20, window.Width - 20, window.Height - 20), new Rect2D(0,1,1,0));

            ctx.Flush();
        }



    }
}
