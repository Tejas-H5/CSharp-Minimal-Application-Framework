﻿using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace MinimalAF.VisualTests.Rendering
{
	public class KeyboardTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Keyboard test";

           SetClearColor(Color4.RGBA(0, 0, 0, 0));
            SetFont("Consolas", 36);
        }

        string KeysToString(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (Input.Keyboard.IsShiftDown)
                {
                    c = char.ToUpper(c);
                }

                sb.Append(CharKeyMapping.CharToString(c));
            }
            return sb.ToString();
        }

        public override void OnRender()
        {
            SetDrawColor(1, 1, 1, 1);

            Text("Press some keys:", Width / 2, Height / 2 + 200);

            Text(KeysToString(Input.Keyboard.CharactersDown), Width / 2, Height / 2);
        }

        public override void OnUpdate()
        {
            if (Input.Keyboard.IsAnyPressed)
            {
                Console.WriteLine("PRessed: " + Input.Keyboard.CharactersPressed);
            }

            if (Input.Keyboard.IsAnyReleased)
            {
                Console.WriteLine("Released: " + Input.Keyboard.CharactersReleased);
            }
        }
    }
}
