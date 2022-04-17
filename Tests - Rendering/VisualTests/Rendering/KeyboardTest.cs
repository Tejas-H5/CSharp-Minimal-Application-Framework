using MinimalAF;
using MinimalAF.Rendering;
using System;
using System.Text;

namespace MinimalAF.VisualTests.Rendering {
    [VisualTest]
    public class KeyboardTest : Element {
        public override void OnMount(Window w) {

            w.Size = (800, 600);
            w.Title = "Keyboard test";

            SetClearColor(Color4.White);
            SetFont("Consolas", 36);
        }

        string KeysToString(string s) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];
                if (KeyHeld(KeyCode.Shift)) {
                    c = char.ToUpper(c);
                }

                sb.Append(CharKeyMapping.CharToString(c));
            }
            return sb.ToString();
        }

        public override void OnRender() {
            SetDrawColor(0, 0, 0, 1);

            Text("Press some keys:", VW(0.5f), VH(0.75f), HorizontalAlignment.Center, VerticalAlignment.Center);

            Text(KeysToString(KeyboardCharactersHeld), Width / 2, Height / 2, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Any)) {
                Console.WriteLine("PRessed: " + KeyboardCharactersPressed);
            }

            if (KeyPressed(KeyCode.Any)) {
                Console.WriteLine("Released: " + KeyboardCharactersReleased);
            }
        }
    }
}
