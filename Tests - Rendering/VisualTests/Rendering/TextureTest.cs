using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF.VisualTests.Rendering
{
	class TextureTest : Element
    {
        Texture _tex;
        Texture _tex2;

        public override void OnStart()
        {
            WindowElement w = GetAncestor<WindowElement>();
            w.Size = (800, 600);
            w.Title = "Texture loading test";
            //w.RenderFrequency = 120; 60;
            //w.UpdateFrequency = 120; 120;

            CTX.SetClearColor(0, 0, 0, 1);

            TextureMap.LoadTexture("placeholder", "./Res/settings_icon.png", new TextureImportSettings());
            TextureMap.LoadTexture("placeholderNN", "./Res/settings_icon.png", new TextureImportSettings { Filtering = FilteringType.NearestNeighbour });

            _tex = TextureMap.GetTexture("placeholder");
            _tex2 = TextureMap.GetTexture("placeholderNN");
        }


        public override void OnUpdate()
        {
            t += (float)Time.DeltaTime;
        }

        float t = 0;

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            CTX.SetTexture(_tex);
            CTX.Rect.Draw(20, 20, Width / 2 - 20, Height - 20);

            Rect2D rect2 = new Rect2D(Width / 2 + 20, 20, Width - 20, Height - 20);

            CTX.PushMatrix(Matrix4.Transpose(Matrix4.CreateTranslation(rect2.CenterX, rect2.CenterY, 0)));
            CTX.PushMatrix(Matrix4.CreateRotationZ(t));
            CTX.PushMatrix(Matrix4.CreateScale(MathF.Sin(t)));

            CTX.SetTexture(_tex2);
            CTX.Rect.Draw(new Rect2D(-rect2.Width / 2, -rect2.Height / 2, rect2.Width / 2, rect2.Height / 2).Rectify());

            CTX.PopMatrix();
            CTX.PopMatrix();
            CTX.PopMatrix();
        }
    }
}
