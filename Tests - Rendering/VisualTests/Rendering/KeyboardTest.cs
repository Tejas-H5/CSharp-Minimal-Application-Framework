using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace RenderingEngineRenderingTests.VisualTests.Rendering
{
    public class KeyboardTest : Element
    {
        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Keyboard test";

            CTX.SetClearColor(0, 0, 0, 0);
            CTX.SetCurrentFont("Consolas", 36);
        }

        string KeysToString(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
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

        public override void OnRender()
        {
            CTX.SetDrawColor(1, 1, 1, 1);

            CTX.DrawText("Press some keys:", Width / 2, Height / 2 + 200);

            CTX.DrawText(KeysToString(Input.CharactersDown), Width / 2, Height / 2);
        }

        public override void OnUpdate()
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
