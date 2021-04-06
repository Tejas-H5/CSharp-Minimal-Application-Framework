using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests
{
    public class KeyboardTest : EntryPoint
    {
        public override void Start()
        {
            Window.Size = (800, 600);
            Window.Title = "Keyboard test";

            CTX.SetClearColor(0, 0, 0, 0);
            CTX.SetCurrentFont("Consolas", 36);
        }

        string _charactersPressed = "";

        string KeysToString(string s)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (Input.IsShiftDown)
                {
                    c = char.ToUpper(c);
                }

                sb.Append(CharKeyMapping.CharToString(c));
            }
            return sb.ToString();
        }

        public override void Render(double deltaTime)
        {
            CTX.Clear();

            CTX.SetDrawColor(1, 1, 1, 1);

            CTX.DrawText("Press some keys:", Window.Width / 2, Window.Height / 2 + 200);

            CTX.DrawText(KeysToString(Input.CharactersDown), Window.Width / 2, Window.Height / 2);


            CTX.Flush();
        }

        public override void Update(double deltaTime)
        {

            if (Input.IsAnyKeyPressed)
            {
                Console.WriteLine("PRessed: " + Input.CharactersPressed);
            }

            if (Input.IsAnyKeyReleased)
            {
                Console.WriteLine("Released: " + Input.CharactersReleased);
            }
        }
    }
}
