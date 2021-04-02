﻿using OpenTK.Mathematics;
using RenderingEngine.Rendering;
using RenderingEngine.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests
{
    //Performs a binary search to see the max number of random lines that can be drawn for 60FPS
    class GeometryAndTextTest : EntryPoint
    {
        List<string> rain;
        Texture _tex;

        public override void Start()
        {
            
            _window.Size=(800, 600);
            _window.Title=("Text and geometry test");
            //window.RenderFrequency = 60;
            //window.UpdateFrequency = 120;

            _ctx.SetClearColor(0, 0, 0, 0);

            _ctx.SetCurrentFont("Consolas", 24);

            _window.MouseWheel += MousewheelScroll;

            rain = new List<string>();

            TextureMap.RegisterTexture("placeholder", "settings_icon.png", new TextureImportSettings
            {
                Filtering = FilteringType.NearestNeighbour
            });

            _tex = TextureMap.GetTexture("placeholder");
        }

        StringBuilder sb = new StringBuilder();
        Random rand = new Random();
        void PushGibberish(RenderingContext ctx)
        {
            sb.Clear();

            float totalLength = 0;
            while (totalLength < _window.Width)
            {
                int character = rand.Next(0, 512);
                char c = (char)character;
                if (character > 126)
                    c = ' ';

                sb.Append(c);

                totalLength += ctx.GetCharWidth(c);
            }

            rain.Insert(0, sb.ToString());
            if ((rain.Count - 2) * ctx.GetCharHeight() > _window.Height)
            {
                rain.RemoveAt(rain.Count - 1);
            }
        }

        private void MousewheelScroll(OpenTK.Windowing.Common.MouseWheelEventArgs obj)
        {
        }


        double timer = 0;
        public override void Update(double deltaTime)
        {
            //*
            timer += deltaTime;
            a += (float)deltaTime;
            if (timer < 0.05)
                return;
            //*/
            timer = 0;

            PushGibberish(_ctx);
        }

        float a = 0;

        public override void Render(double deltaTime)
        {
            _ctx.Clear();

            _ctx.SetDrawColor(0, 1, 0, 0.8f);
            _ctx.SetTexture(null);

            for (int i = 0; i < rain.Count; i++)
            {
                _ctx.DrawText(rain[i], 0, _window.Height - _ctx.GetCharHeight() * i);
            }

            
            _ctx.DrawFilledArc(_window.Width / 2, _window.Height / 2, MathF.Min(_window.Height / 2f,_window.Width / 2f), a, MathF.PI*2 + a, 6);

            _ctx.SetTexture(_tex);
            //ctx.DrawFilledArc(window.Width / 2, window.Height / 2, MathF.Min(window.Height / 2f, window.Width / 2f)/2f, a/2f, MathF.PI * 2 + a/2f, 6);

            _ctx.DrawRect(_window.Width / 2 - 50, _window.Height / 2 - 50, _window.Width / 2 + 50, _window.Height / 2 + 50);

            //ctx.DrawRect(100,100,window.Width-100, window.Height-100);
            _ctx.Flush();
        }



        public override void Cleanup()
        {
            base.Cleanup();

        }
    }
}
