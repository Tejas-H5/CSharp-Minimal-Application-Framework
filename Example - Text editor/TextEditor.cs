using MinimalAF;

namespace TextEditor {
    class TextEditor : IRenderable {
        TextBuffer _buffer;

        // offset from start of bufferer where we are inserting/removing characters
        int _cursorPos = 0;

        // scrolling
        float _wantedScrollPos = 0;
        float _currentScroll = 0;

        // selection
        int _selectStartPos;
        int _selectEndPos;
        bool _isSelecting;
        // we need to be able to clear the selection when the buffer length changes, because 
        // the selected region becomes invalid. I.E When I have [10, 20] selected but I add a new character before index 10, 
        // the entire selection has 'moved bacwards' (well it hasn't moved a all, but rather all the text has moved forwards.
        // An approach is to invoke an OnTextBufferEdit event each time the textbuffer is edited, but I am too lazy to implement that at the moment
        // (yeah, I wrote this long ass comment but didn't add that thing)).
        int _bufferLengthWhenSelectionWasMade;

        bool HasSelection => _selectEndPos != _selectStartPos;

        List<Rect> _highlightRegions = new List<Rect>();


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

        void RenderText(FrameworkContext ctx) {
            float padding = 10;

            ctx.SetFont("Source code pro", 24);
            ctx.SetDrawColor(AppColors.FG);
            ctx.SetTextureToCurrentFontTexture();

            // TODO: scrolling
            int currentLineNumber = 0;
            float charHeight = ctx.GetCharHeight();
            float charWidth = ctx.GetCharWidth();
            float documentY = 0;

            // _currentScroll = MathHelpers.Lerp(_currentScroll, _wantedScrollPos, 50 * Time.DeltaTime);
            _currentScroll = _wantedScrollPos;

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
                ctx.SetDrawColor(Color.Blue, 0.4f);

                // render the higlight regions we found in the previous frame
                {
                    if (HasSelection) {
                        for(int i = 0; i < _highlightRegions.Count; i++) {
                            var region = _highlightRegions[i];
                            ctx.DrawRect(region);
                        }
                    }
                    _highlightRegions.Clear();
                }

                ctx.DrawLine(dividerLineX, 0, dividerLineX, ctx.VH, 1);
                ctx.SetTextureToCurrentFontTexture();
                ctx.SetDrawColor(Color.Black);

                // render the text
                {
                    x = startX;
                    for (int i = startPos; i <= _buffer.Length; documentY -= charHeight) {
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
                        for (; i <= _buffer.Length; i++) {
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
                                        float y = relativeY();
                                        var s = ctx.DrawChar(c, x, y);

                                        // set up a highlight region if applicable
                                        int minSel = MathHelpers.Min(_selectStartPos, _selectEndPos);
                                        int maxSel = MathHelpers.Max(_selectStartPos, _selectEndPos);
                                        if (HasSelection && (i >= minSel && i < maxSel)) {
                                            _highlightRegions.Add(new Rect(x, y, x + s.X, y + s.Y));
                                        }

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
                                ctx.SetTextureToCurrentFontTexture();

                                _wantedScrollPos = documentY;
                            }

                            if (lineEnded) {
                                i++;
                                break;  // breaking here should increment and draw a line number
                            }
                        }
                    }
                }
            }

            Update(ref ctx);
        }

        void CancelSelection() {
            // _cursorPos as the value to set here is not an arbitrary decision. It allows selections to work as expected when 
            // Typing capital letters while holding shift, and then moving around while still holding down shift
            _selectStartPos = _cursorPos;
            _selectEndPos = _cursorPos;
            _bufferLengthWhenSelectionWasMade = _buffer.Length;
        }

        int AddLetterAtPosition(int pos, char c) {
            if (HasSelection) {
                pos = DeleteSelection(_selectStartPos, _selectEndPos);
            }

            return _buffer.AddLetterAtCursor(pos, c);
        }

        int DeleteSelection(int start, int end) {
            _buffer.RemoveRange(start, end);
            CancelSelection();

            return MathHelpers.Min(start, end);
        }

        void Update(ref FrameworkContext ctx) {
            // selection part one
            {
                _isSelecting = ctx.KeyIsDown(KeyCode.Shift);
                if (ctx.KeyJustPressed(KeyCode.Shift)) {
                    _selectStartPos = _cursorPos;
                    // we are setting this before we handle keyboard input, which is when we could add text to the buffer, thereby making
                    // the selection invalid. And I can add the selection invalidation check after we handle keyboard input
                    _bufferLengthWhenSelectionWasMade = _buffer.Length;
                }
                if (_isSelecting) {
                    _selectEndPos = _cursorPos;
                }
            }


            // process repeating keyboard inputs
            {
                for (int i = 0; i < ctx.RepeatableKeysInput.Count; i++) {
                    var rKey = ctx.RepeatableKeysInput[i];

                    if (rKey.Type == RepeatableKeyboardInputType.TextInput) {
                        _cursorPos = AddLetterAtPosition(_cursorPos, (char)rKey.TextInput);
                        continue;
                    }

                    if (rKey.Type == RepeatableKeyboardInputType.KeyboardInput) {
                        var key = rKey.KeyCode;
                        if (key == KeyCode.Enter) {
                            _cursorPos = AddLetterAtPosition(_cursorPos, '\n');
                            continue;
                        }

                        if (key == KeyCode.Backspace) {
                            if (HasSelection) {
                                _cursorPos = DeleteSelection(_selectStartPos, _selectEndPos);
                            } else if (ctx.KeyIsDown(KeyCode.Control)) {
                                // remove an entire word
                                int toPrevWord = _buffer.MoveCursorBackAWord(_cursorPos);
                                _buffer.RemoveRange(toPrevWord, _cursorPos);
                                _cursorPos = toPrevWord;
                            } else {
                                // remove just a letter
                                _cursorPos = _buffer.BackspaceLetter(_cursorPos);
                            }
                            continue;
                        }

                        if (key == KeyCode.Tab) {
                            _cursorPos = AddLetterAtPosition(_cursorPos, '\t');
                            continue;
                        }

                        _cursorPos = MoveCursorBasedOnKeyCodeForTypicalMovement(
                            rKey.KeyCode, _cursorPos, ctx.KeyIsDown(KeyCode.Control)
                        );
                        continue;
                    }
                }
            }

            // process normal game-style keyboard inputs
            {
                // selection part two
                {
                    bool shouldCancelSelection = ctx.KeyJustPressed(KeyCode.Escape) || _bufferLengthWhenSelectionWasMade != _buffer.Length;
                    if (shouldCancelSelection) {
                        CancelSelection();
                    }
                }
            }

            // process scrolling the mosuewheel
            {
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


        // A problem with C# is that to get aesthetic looking switch statements you
        // need to extract functions
        int MoveCursorBasedOnKeyCodeForTypicalMovement(KeyCode key, int pos, bool hasCtrl) {
            if (hasCtrl) {
                switch (key) {
                    case KeyCode.Left:
                        return _buffer.MoveCursorBackAWord(pos);
                    case KeyCode.Right:
                        return _buffer.MoveCursorForwardsAWord(pos);
                    case KeyCode.End:
                        return _buffer.ClampCursorPosition(_buffer.Length);
                    case KeyCode.Home:
                        return 0;
                }

                return pos;
            }

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
    }
}
