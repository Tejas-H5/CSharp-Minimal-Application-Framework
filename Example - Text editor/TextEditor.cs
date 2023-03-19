using MinimalAF;

namespace TextEditor {


    class TextEditor : IRenderable {
        // offset from start of bufferer where we are inserting/removing characters
        int _cursorPos = 0;
        TextBuffer _buffer;
        float _wantedScrollPos = 0;
        float _currentScroll = 0;

        public TextEditor() {
            // _buffer = new TextBuffer("");
            _buffer = new TextBuffer(
                TestData.TextTextCSharpCodeIDKwhereitsfromguysWhatCouldItBe
                    .Replace("\r", "")
            );
        }

        public void Render(FrameworkContext ctx) {
            float margin = 10;
            float pageWidth = ctx.VW;
            var pageRect = new Rect(
                margin, margin,
                ctx.VW - margin, ctx.VH - margin
            );

            ctx.SetDrawColor(Color.Black);
            ctx.DrawRectOutline(2, pageRect);

            RenderText(
                ctx.WithRect(pageRect, true).Use()
            );
        }

        float lerp(float a, float b, float t) {
            return a + (b - a) * t;
        }

        void RenderText(FrameworkContext ctx) {
            float padding = 10;

            ctx.SetFont("Source code pro", 24);
            ctx.SetDrawColor(AppColors.FG);
            ctx.SetTexture(ctx.InternalFontTexture);

            // TODO: scrolling
            int currentLineNumber = 0;
            float charHeight = ctx.GetCharHeight();
            float charWidth = ctx.GetCharWidth();
            float documentY = 0;

            _currentScroll = lerp(_currentScroll, _wantedScrollPos, 20 * Time.DeltaTime);

            var relativeY = () => (documentY + ctx.VH * 0.5f - _currentScroll);
            int startPos = 0;

            // float startY = documentY;
            // int startLineNumber = currentLineNumber;

            // TODO: remove allocation. Need to update text API to support MutableString
            int totalLineCount = _buffer.CountOccurrances("\n");
            float lineNumberMargin = ctx.GetStringWidth(totalLineCount.ToString()) + 2 * padding;

            // render all the text
            {
                float startX = lineNumberMargin;
                float x = startX;
                float wrapMargin = 100;

                // line between line numbers and text
                float dividerLineX = lineNumberMargin - padding / 2;
                ctx.SetTexture(null);
                ctx.DrawLine(dividerLineX, 0, dividerLineX, ctx.VH, 1);
                ctx.SetTexture(ctx.InternalFontTexture);

                // start rendering the text
                {
                    x = startX;
                    for (int i = startPos; i <= _buffer.Length && relativeY() >= -charHeight; documentY -= charHeight) {
                        // draw line numbers here itself.
                        // the text may wrap, causing incorrect line numbers if we draw them in 
                        // a seperate loop
                        {
                            float relY = relativeY();
                            if (relY > -charHeight && relY < ctx.VH) {
                                // TODO: stop allocation
                                ctx.DrawText(currentLineNumber.ToString(), padding, relY);
                            }
                            currentLineNumber++;
                        }

                        // draw text on this line. It may wrap - 
                        // <= is so we can draw the cursor when it is at the very end
                        for (; i <= _buffer.Length && relativeY() >= -charHeight; i++) {
                            float characterStartX = x, characterStartY = relativeY();
                            bool lineEnded = false;

                            // draw a single thing
                            if (i < _buffer.Length) {
                                var c = _buffer[i];
                                if (c == '\n') {
                                    // draw newline
                                    x = startX;
                                    lineEnded = true;
                                } else {
                                    // draw character
                                    float relY = relativeY();
                                    if (relY > -charHeight && relY < ctx.VH) {
                                        var s = ctx.DrawChar(c, x, relativeY());
                                        x += s.X;
                                    } else {
                                        x += ctx.GetCharWidth(c);
                                    }

                                    // TODO: wrap words somehow instead of characters. would involve
                                    // some sort of lookahead I think
                                    if (x + charWidth > ctx.VW) {
                                        x = lineNumberMargin + wrapMargin;
                                        documentY -= charHeight;
                                    }
                                }
                            }

                            // draw cursor if it is at this position
                            if (_cursorPos == i) {
                                var cursorWidth = 3;
                                ctx.SetTexture(null);
                                ctx.DrawRect(characterStartX, characterStartY, characterStartX + cursorWidth, characterStartY + charHeight);
                                ctx.SetTexture(ctx.InternalFontTexture);

                                _wantedScrollPos = documentY;
                            }

                            if (lineEnded) {
                                i++;
                                break;  // so we can get a line number
                            }
                        }
                    }
                }
            }

            ProcessInputs(ref ctx);
        }

        // A problem with C# is that to get aesthetic looking switch statements you
        // need to extract functions
        int MoveCursorBasedOnKeyCodeForMovementMode(KeyCode key, int pos) {
            switch (key) {
                case KeyCode.J:
                    return _buffer.ClampCursorPosition(pos - 1);
                case KeyCode.L:
                    return _buffer.ClampCursorPosition(pos + 1);
                case KeyCode.I:
                    return _buffer.MoveCursorUpALine(pos);
                case KeyCode.K:
                    return _buffer.MoveCursorDownALine(pos);
                case KeyCode.O:
                    return _buffer.MoveCursorToEndOfLine(pos);
                case KeyCode.U:
                    return _buffer.MoveCursorToHomeOfLine(pos);
                case KeyCode.D:
                    return _buffer.MoveCursorToEndOfNextWord(pos);
                case KeyCode.A:
                    return _buffer.MoveCursorToEndOfPreviousWord(pos);
            }

            return pos;
        }

        void ProcessInsertModeInputs(ref FrameworkContext ctx) {
            for (int i = 0; i < ctx.RepeatableKeysInput.Count; i++) {
                var rKey = ctx.RepeatableKeysInput[i];

                if (rKey.Type == RepeatableKeyboardInputType.TextInput) {
                    _cursorPos = _buffer.AddLetterAtCursor(_cursorPos, (char)rKey.TextInput);
                    continue;
                }

                if (rKey.Type == RepeatableKeyboardInputType.KeyboardInput) {
                    var key = rKey.KeyCode;
                    if (key == KeyCode.Enter) {
                        _cursorPos = _buffer.AddLetterAtCursor(_cursorPos, '\n');
                        continue;
                    }

                    if (key == KeyCode.Backspace) {
                        _cursorPos = _buffer.AddLetterAtCursor(_cursorPos, '\b');
                        continue;
                    }

                    if (key == KeyCode.Tab) {
                        _cursorPos = _buffer.AddLetterAtCursor(_cursorPos, '\t');
                        continue;
                    }
                }
            }

            ProcessTypicalMovement(ref ctx);
        }


        // A problem with C# is that to get aesthetic looking switch statements you
        // need to extract functions
        int MoveCursorBasedOnKeyCodeForTypicalMovement(KeyCode key, int pos) {
            switch (key) {
                case KeyCode.Left:
                    return _buffer.ClampCursorPosition(pos - 1);
                case KeyCode.Right:
                    return _buffer.ClampCursorPosition(pos + 1);
                case KeyCode.Up:
                    return _buffer.MoveCursorUpALine(pos);
                case KeyCode.Down:
                    return _buffer.MoveCursorDownALine(pos);
                case KeyCode.End:
                    return _buffer.MoveCursorToEndOfLine(pos);
                case KeyCode.Home:
                    return _buffer.MoveCursorToHomeOfLine(pos);
            }

            return pos;
        }


        void ProcessTypicalMovement(ref FrameworkContext ctx) {
            for(int i = 0; i < ctx.RepeatableKeysInput.Count; i++) {
                var rKey = ctx.RepeatableKeysInput[i];
                if (rKey.Type == RepeatableKeyboardInputType.TextInput) continue;

                _cursorPos = MoveCursorBasedOnKeyCodeForTypicalMovement(rKey.KeyCode, _cursorPos);
            }
        }

        void ProcessInputs(ref FrameworkContext ctx) {
            ProcessInsertModeInputs(ref ctx);

            Console.WriteLine(ctx.MouseWheelNotches);
            if (ctx.MouseWheelNotches > 0.5f) {
                for (int i = 0; i < 5; i++) {
                    _cursorPos = _buffer.MoveCursorUpALine(_cursorPos);
                }
            } else if (ctx.MouseWheelNotches < -0.5f) {
                for (int i = 0; i < 5; i++) {
                    _cursorPos = _buffer.MoveCursorDownALine(_cursorPos);
                }
            }
        }
    }
}
