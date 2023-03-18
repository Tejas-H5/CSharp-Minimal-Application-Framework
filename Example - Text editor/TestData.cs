namespace TextEditor {
    public static class TestData {
        public const string TextTextCSharpCodeIDKwhereitsfromguysWhatCouldItBe = @"
void RenderText(ref FrameworkContext ctx) {
        ctx.SetFont(""Source code pro"", 24);
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
        for (int i = 0; i <= _buffer.Length; i++) {
            float characterStartX = x;
            if (i < _buffer.Length) {
                // draw text
                var c = _buffer[i];

                if (c == '\n') {
                    x = startX;
                    y -= charHeight;
                    continue;
                }

                var s = ctx.DrawChar(c, x, y);
                x += s.X;
            }

            // draw cursor if it is at this position
            if (cursorPos == i) {
                var cursorWidth = 3;

                ctx.SetTexture(null);
                ctx.DrawRect(characterStartX, y, characterStartX + cursorWidth, y + ctx.GetCharHeight('|'));
                ctx.SetTexture(ctx.InternalFontTexture);
            }
        }
    }

    void ProcessMovementModeInputs(ref FrameworkContext ctx) {
        for (int i = 0; i < ctx.StruckKeys.Count; i++) {
            var key = ctx.StruckKeys[i];

            if (key == KeyCode.A) {
                cursorPos = _buffer.MoveCursorToPreviousWord(cursorPos);
            } else if (key == KeyCode.D) {
                cursorPos = _buffer.MoveCursorToNextWord(cursorPos);
            }
        }

        ProcessTypicalMovement(ref ctx);
    }

    void ProcessInsertModeInputs(ref FrameworkContext ctx) {
        for (int i = 0; i < ctx.StruckKeys.Count; i++) {
            var key = ctx.StruckKeys[i];

            var (keyCharacter, found) = key.ToChar();
            if (found) {
                if (ctx.KeyIsDown(KeyCode.Shift)) {
                    keyCharacter = char.ToUpper(keyCharacter);
                }

                cursorPos = _buffer.AddLetterAtCursor(cursorPos, keyCharacter);
                continue;
            }
        }

        ProcessTypicalMovement(ref ctx);
    }
";

    }
}
