using MinimalAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor {
    internal class TextArea : IRenderable {
        public bool LineNumbers = true;
        public bool AcceptsNewLines = true;


        TextBuffer _buffer;
        bool _bufferContentsChanged;

        // offset from start of bufferer where we are inserting/removing characters
        int _cursorPos = 0;

        // scrolling
        float _wantedScrollPos = 0;
        float _currentScroll = 0;

        // selection
        int _selectStartPos;
        int _selectEndPos;
        bool _isSelecting;

        bool HasSelection => _selectEndPos != _selectStartPos;


        public string GetText() {
            return _buffer.BuildString();
        }

        public void Clear() {
            _buffer.Clear();
            _cursorPos = 0;
            CancelSelection();
        }

        List<Rect> _highlightRegions = new List<Rect>();

        public TextArea(string initialText) {
            // _buffer = new TextBuffer("");
            _buffer = new TextBuffer(initialText.Replace("\r", ""));
            _buffer.TextEdited += _buffer_TextEdited;
        }

        private void _buffer_TextEdited() {
            _bufferContentsChanged = true;
        }

        public void Render(FrameworkContext ctx) {
            RenderText(ctx, true);
        }

        public int GoToLine(int line) {
            int linePos = _buffer.GetLinePos(line);
            if (linePos != -1) {
                _cursorPos = linePos;
                return linePos;
            }

            return -1;
        }

        public void RenderText(FrameworkContext ctx, bool update) {
            float padding = 10;

            ctx.SetFont(AppConfig.EditorFont, AppConfig.FontSize1);
            ctx.SetDrawColor(AppConfig.FG);
            ctx.SetTexture(ctx.CurrentFontTexture);

            // TODO: scrolling
            int currentLineNumber = 0;
            float charHeight = ctx.GetCharHeight();
            float charWidth = ctx.GetCharWidth();
            float documentY = 0;

            _currentScroll = MathHelpers.Lerp(_currentScroll, _wantedScrollPos, 40 * Time.DeltaTime);
            // _currentScroll = _wantedScrollPos;

            var relativeY = () => (documentY + ctx.VH * 0.5f - _currentScroll);
            int startPos = 0;

            // float startY = documentY;
            // int startLineNumber = currentLineNumber;

            float lineNumberMargin = padding;
            if (LineNumbers) {
                int totalLineCount = _buffer.CountOccurrances("\n");
                // the lengths I go to avoid allocation :(
                lineNumberMargin = ctx.GetStringWidth("0") * (1 + MathF.Floor(MathF.Log10(totalLineCount)))
                    + 2 * padding;
            } 

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
                        for (int i = 0; i < _highlightRegions.Count; i++) {
                            var region = _highlightRegions[i];
                            ctx.DrawRect(region);
                        }
                    }
                    _highlightRegions.Clear();
                }

                ctx.SetDrawColor(AppConfig.FG);
                ctx.DrawLine(dividerLineX, 0, dividerLineX, ctx.VH, 1);
                ctx.SetTexture(ctx.CurrentFontTexture);

                // render the text
                {
                    x = startX;
                    for (int i = startPos; i <= _buffer.Length; documentY -= charHeight) {
                        // draw line numbers here itself.
                        // the text may wrap, causing incorrect line numbers if we draw them in 
                        // a seperate loop
                        if (LineNumbers) {
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
                            float y = characterStartY;
                            bool lineEnded = false;

                            // draw a single thing
                            if (i < _buffer.Length) {
                                var c = _buffer[i];
                                if (c == '\n') {
                                    // draw newline
                                    x = startX;
                                    lineEnded = true;

                                    // set up a highlight region if applicable for the newline
                                    int minSel = MathHelpers.Min(_selectStartPos, _selectEndPos);
                                    int maxSel = MathHelpers.Max(_selectStartPos, _selectEndPos);
                                    float newlineHighlightWidth = 10;
                                    if (HasSelection && (i >= minSel && i < maxSel)) {
                                        _highlightRegions.Add(new Rect(
                                            characterStartX, characterStartY, characterStartX + newlineHighlightWidth, characterStartY + charHeight
                                        ));
                                    }
                                } else {
                                    // draw character
                                    float relY = relativeY();
                                    if (relY > -charHeight && relY < ctx.VH) {
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
                                ctx.SetTexture(ctx.CurrentFontTexture);

                                _wantedScrollPos = documentY + charHeight / 2.0f;
                            }

                            if (lineEnded) {
                                i++;
                                break;  // breaking here should increment and draw a line number
                            }
                        }
                    }
                }
            }

            if (update) {
                Update(ref ctx);
            }
        }


        void Update(ref FrameworkContext ctx) {
            int bufferLengthBeforeCommands = _buffer.Length;
            int cursorPosBeforeCommands = _cursorPos;

            // selection part one
            {
                _isSelecting = ctx.KeyIsDown(KeyCode.Shift);
                if (ctx.KeyJustPressed(KeyCode.Shift)) {
                    _selectStartPos = _cursorPos;
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

            // Cancel the selection if we have inserted something or if we have moved.
            // If we insert something, the bounds become invalid.
            // If we move, then when we start typing again, we start adding stuff at the place we highlighted.
            // Fixing these problems takes a lot of effort or causes more problems IMO
            {
                if (_bufferContentsChanged) {
                    _bufferContentsChanged = false;
                    CancelSelection();
                }

                if (!_isSelecting && cursorPosBeforeCommands != _cursorPos) {
                    CancelSelection();
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

        void CancelSelection() {
            // _cursorPos as the value to set here is not an arbitrary decision. It allows selections to work as expected when 
            // Typing capital letters while holding shift, and then moving around while still holding down shift
            _selectStartPos = _cursorPos;
            _selectEndPos = _cursorPos;
        }

        int AddLetterAtPosition(int pos, char c) {
            if (!AcceptsNewLines && c == '\n') return pos;

            if (HasSelection) {
                pos = DeleteSelection(_selectStartPos, _selectEndPos);
            }

            if (c == '\t') {
                // insert spaces instead of tabs for now.
                // Our rendering engine has hardcoded tab-stops to be 4, but we need to make
                // this settable via the framework. This also means that all distance measuring functions
                // in the TextBuffer are miss-counting a tab as just 1 index instead of 4 or however many spaces are 1 tab.

                int lineStart = _buffer.MoveCursorToLineStart(pos);
                int distanceFromLine = pos - lineStart;
                int tabStopLength = 4;  // Eventually will be driven by editor settings or something
                int distanceToNextTabStop = tabStopLength - (distanceFromLine % tabStopLength);

                for (int i = 0; i < distanceToNextTabStop; i++) {
                    // TODO: We should only be resizing the buffer once. 
                    // right now, AddLetterAtCursor will move all chars to the right by 1,
                    // so we are doing this distanceToNextTabStop times.
                    // We probably need a temporary MutableString where we can construct the tabs and copy them
                    // over or something. Or maybe augment addLetterAtPosition to take a repeat count


                    pos = _buffer.AddLetterAtCursor(pos, ' ');
                }
                return pos;
            }


            return _buffer.AddLetterAtCursor(pos, c);
        }

        int DeleteSelection(int start, int end) {
            _buffer.RemoveRange(start, end);
            CancelSelection();

            return MathHelpers.Min(start, end);
        }
    }
}
