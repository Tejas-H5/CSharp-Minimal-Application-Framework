using MinimalAF;

namespace TextEditor {


    class TextEditor : IRenderable {
        // offset from start of bufferer where we are inserting/removing characters
        int _cursorPos = 0;
        TextBuffer _buffer;
        float _lastCursorPosY = 0;

        public TextEditor() {
            // _buffer = new TextBuffer();
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
            var relativeY = () => (documentY + ctx.VH * 0.5f - _lastCursorPosY);
            int startPos = 0;

            float startY = documentY;
            int startLineNumber = currentLineNumber;
            int maxLineNumber = currentLineNumber + (int)Math.Ceiling(ctx.VH / charHeight);
            // TODO: remove allocation. Need to update text API to support MutableString
            float lineNumberMargin = ctx.GetStringWidth(maxLineNumber.ToString()) + 2 * padding;

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

                                _lastCursorPosY = documentY;
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

        void ProcessMovementModeInputs(ref FrameworkContext ctx) {
            for (int i = 0; i < ctx.RepeatableKeysInput.Count; i++) {
                var rKey = ctx.RepeatableKeysInput[i];
                if (rKey.Type == RepeatableKeyboardInputType.TextInput) continue;

                var key = rKey.KeyCode;
                if (key == KeyCode.A) {
                    _cursorPos = _buffer.MoveCursorToEndOfPreviousWord(_cursorPos);
                } else if (key == KeyCode.D) {
                    _cursorPos = _buffer.MoveCursorToEndOfNextWord(_cursorPos);
                } else if (rKey.KeyCode == KeyCode.W) {
                    _cursorPos = _buffer.MoveCursorUpALine(_cursorPos);
                } else if (rKey.KeyCode == KeyCode.S) {
                    _cursorPos = _buffer.MoveCursorDownALine(_cursorPos);
                }
            }

            ProcessTypicalMovement(ref ctx);
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

        void ProcessTypicalMovement(ref FrameworkContext ctx) {
            for(int i = 0; i < ctx.RepeatableKeysInput.Count; i++) {
                var rKey = ctx.RepeatableKeysInput[i];
                if (rKey.Type == RepeatableKeyboardInputType.TextInput) continue;

                if (rKey.KeyCode == KeyCode.Left) {
                    _cursorPos = _buffer.MoveCursor(_cursorPos - 1);
                } else if (rKey.KeyCode == KeyCode.Right) {
                    _cursorPos = _buffer.MoveCursor(_cursorPos + 1);
                } else if (rKey.KeyCode == KeyCode.Up) {
                    _cursorPos = _buffer.MoveCursorUpALine(_cursorPos);
                } else if (rKey.KeyCode == KeyCode.Right) {
                    _cursorPos = _buffer.MoveCursorDownALine(_cursorPos);
                }
            }
        }

        void ProcessInputs(ref FrameworkContext ctx) {
            if (ctx.KeyIsDown(KeyCode.Control)) {
                ProcessMovementModeInputs(ref ctx);
                return;
            }

            ProcessInsertModeInputs(ref ctx);
        }
    }
}
