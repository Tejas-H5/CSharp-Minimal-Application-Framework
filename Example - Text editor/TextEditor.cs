using MinimalAF;

namespace TextEditor {
    class TextEditor : IRenderable {
        MutableString buffer = new MutableString();

        // offset from start of bufferer where we are inserting/removing characters
        int cursorPosition = 0;

        void AddLetterAtCursor(char c) {
            if (c == '\b') {
                // we really want to remove a character that is behind the cursor
                buffer.Remove(cursorPosition - 1);
                MoveCursorBackwards(1);
                return;
            }

            buffer.Insert(c, cursorPosition);
            MoveCursorForwards(1);
        }

        void MoveCursorBackwards(int amonut) {
            cursorPosition -= amonut;
            if (cursorPosition < 0) cursorPosition = 0;
        }


        void MoveCursorForwards(int amount) {
            cursorPosition += amount;

            // not an off-by-one-error. cursorPosition === buffer.Length is for appending
            if (cursorPosition > buffer.Length) cursorPosition = buffer.Length;
        }

        public void Render(FrameworkContext ctx) {
            RenderText(ref ctx);

            ProcessInputs(ref ctx);
        }

        void RenderText(ref FrameworkContext ctx) {
            ctx.SetFont("Source code pro", 24);
            ctx.SetDrawColor(AppColors.FG);

            // TODO: clean this up
            var previousTexture = ctx.GetTexture();
            ctx.SetTexture(ctx.InternalFontTexture);

            // render the text
            float startX = 100;
            float charHeight = ctx.GetCharHeight();
            float x = startX;
            float y = ctx.VH - 100;
            // <= is so we can draw the cursor when it is at the very end
            for (int i = 0; i <= buffer.Length; i++) {
                float characterStartX = x;
                if (i < buffer.Length) {
                    // draw text
                    var c = buffer[i];

                    if (c == '\n') {
                        x = startX;
                        y -= charHeight;
                        continue;
                    }

                    var s = ctx.DrawChar(c, x, y);
                    x += s.X;
                }

                // draw cursor if it is at this position
                if (cursorPosition == i) {
                    var cursorWidth = 3;

                    ctx.SetTexture(null);
                    ctx.DrawRect(characterStartX, y, characterStartX + cursorWidth, y + ctx.GetCharHeight('|'));
                    ctx.SetTexture(ctx.InternalFontTexture);
                }
            }
        }

        void ProcessInputs(ref FrameworkContext ctx) {
            for (int i = 0; i < ctx.StruckKeys.Count; i++) {
                var key = ctx.StruckKeys[i];
                var (keyCharacter, found) = key.ToChar();
                if (found) {
                    if (ctx.KeyIsDown(KeyCode.Shift)) {
                        keyCharacter = char.ToUpper(keyCharacter);
                    }

                    AddLetterAtCursor(keyCharacter);
                    continue;
                }

                if (key == KeyCode.Left) {
                    MoveCursorBackwards(1);
                    continue;
                }

                if (key == KeyCode.Right) {
                    MoveCursorForwards(1);
                    continue;
                }
            }

        }
    }
}
