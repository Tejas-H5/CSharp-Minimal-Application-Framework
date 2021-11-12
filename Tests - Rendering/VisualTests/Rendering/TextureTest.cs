using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF.VisualTests.Rendering
{
    class TextureTest : EntryPoint
    {
        Texture _tex;
        Texture _tex2;

        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Texture loading test";
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            CTX.SetClearColor(0, 0, 0, 1);

            TextureMap.LoadTexture("placeholder", "./Res/settings_icon.png", new TextureImportSettings());
            TextureMap.LoadTexture("placeholderNN", "./Res/settings_icon.png", new TextureImportSettings { Filtering = FilteringType.NearestNeighbour });

            _tex = TextureMap.GetTexture("placeholder");
            _tex2 = TextureMap.GetTexture("placeholderNN");
        }


        public override void Update(double deltaTime)
        {
            t += (float)deltaTime;
        }

        float t = 0;

        public override void Render(double deltaTime)
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            CTX.SetTexture(_tex);
            CTX.DrawRect(20, 20, Window.Width / 2 - 20, Window.Height - 20);

            Rect2D rect2 = new Rect2D(Window.Width / 2 + 20, 20, Window.Width - 20, Window.Height - 20);

            CTX.PushMatrix(Matrix4.Transpose(Matrix4.CreateTranslation(rect2.CenterX, rect2.CenterY, 0)));
            CTX.PushMatrix(Matrix4.CreateRotationZ(t));
            CTX.PushMatrix(Matrix4.CreateScale(MathF.Sin(t)));

            CTX.SetTexture(_tex2);
            CTX.DrawRect(new Rect2D(-rect2.Width / 2, -rect2.Height / 2, rect2.Width / 2, rect2.Height / 2).Rectify());

            CTX.PopMatrix();
            CTX.PopMatrix();
            CTX.PopMatrix();
        }
    }
}
