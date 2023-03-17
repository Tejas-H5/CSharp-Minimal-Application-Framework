using MinimalAF;
using OpenTK.Mathematics;

namespace TextEditor {

    static class AppColors {
        public static Color BG => Color.White;
        public static Color FG => Color.Black;
    }


    class TextEditor : IRenderable {
        MutableString buffer = new MutableString();
        int cursorPositionFromTheEnd = 0;

        void ClampCursor() {
            if (cursorPositionFromTheEnd < 0) cursorPositionFromTheEnd = 0;
            if (cursorPositionFromTheEnd >= buffer.Length) cursorPositionFromTheEnd = buffer.Length - 1;
        }

        void AddLetterAtCursor(char c) {
            buffer.Insert(c, buffer.Length - cursorPositionFromTheEnd);
        }

        public void Render(FrameworkContext ctx) {
            ctx.SetFont("Source code pro", 24);
            ctx.SetDrawColor(AppColors.FG);

            // TODO: clean this up
            var previousTexture = ctx.GetTexture();
            ctx.SetTexture(ctx.InternalFontTexture);

            // render the text
            float x = 100;
            float y = ctx.VH - 100;
            {
                // <= is so we can draw the cursor when it is at the very end
                for (int i = 0; i <= buffer.Length; i++) {
                    if (i < buffer.Length) {
                        // draw text

                        var c = buffer[i];
                        var s = ctx.DrawChar(c, x, y);
                        x += s.X;
                    }

                    // draw cursor if it is here
                    int distFromEnd = buffer.Length - i;
                    if (distFromEnd == cursorPositionFromTheEnd) {
                        var cursorWidth = 3;

                        ctx.SetTexture(null);
                        ctx.DrawRect(x, y, x + cursorWidth, y + ctx.GetCharHeight('|'));
                        ctx.SetTexture(ctx.InternalFontTexture);
                    }
                }
            }

            ctx.SetTexture(previousTexture);

            // process keyboard inputs
            {
                for(int i = 0; i < ctx.Window.TextInput.Length; i++) {
                    char c = ctx.Window.TextInput[i];
                    AddLetterAtCursor(c);
                }

                if (ctx.KeyJustPressed(KeyCode.Enter)) {
                    AddLetterAtCursor('\n');
                } 
                if (ctx.KeyJustPressed(KeyCode.Left)) {
                    cursorPositionFromTheEnd += 1;
                }
                if (ctx.KeyJustPressed(KeyCode.Left)) {
                    cursorPositionFromTheEnd -= 1;
                }

                // TODO: backspace
                // TODO: up a line (HOW WE gonna do that one xd)
            }
        }
    }


    class Program {
        static void Main(string[] args) {
            new ProgramWindow((FrameworkContext ctx) => {
                ctx.SetClearColor(AppColors.BG);
                ctx.Window.Title = "Text editor";
                ctx.Window.SetWindowState(WindowState.Maximized);

                ctx.Window.RenderFrequency = 60;
                // ctx.Window.UpdateFrequency = 0;

                return new TextEditor();
            }).Run();
        }
    }
}
