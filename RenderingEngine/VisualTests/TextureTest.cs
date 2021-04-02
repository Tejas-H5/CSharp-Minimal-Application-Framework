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

        public override void Start()
        {
			
            _window.Size=(800, 600);
            _window.Title=("Texture loading test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            _ctx.SetClearColor(0, 0, 0, 1);

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
            _ctx.Clear();

            _ctx.SetDrawColor(1,1,1,1);

            _ctx.SetTexture(_tex);
            _ctx.DrawRect(20, 20, _window.Width/2 - 20, _window.Height - 20);

            _ctx.SetTexture(_tex2);
            _ctx.DrawRect(new Rect2D(_window.Width / 2 + 20, 20, _window.Width - 20, _window.Height - 20), new Rect2D(0,1,1,0));

            _ctx.Flush();
        }



    }
}
