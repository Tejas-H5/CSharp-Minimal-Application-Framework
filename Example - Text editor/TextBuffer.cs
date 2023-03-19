using MinimalAF;

namespace TextEditor {
    class TextBuffer {
        MutableString _buffer = new MutableString();
        public MutableString Buffer => _buffer;
        public int Length => _buffer.Length;
        public char this[int pos] => _buffer[pos];

        public TextBuffer(string initText) {
            _buffer.Resize(initText.Length);
            for(int i = 0; i < initText.Length; i++) {
                _buffer[i] = initText[i];
            }
        }

        public int ClampCursorPosition(int newPosition) {
            if (newPosition < 0) newPosition = 0;
            if (newPosition > _buffer.Length) newPosition = _buffer.Length;
            return newPosition;
        }

        public int AddLetterAtCursor(int cursorPosition, char c) {
            if (c == '\r') return cursorPosition;

            if (c == '\b') {
                // we really want to remove a character that is behind the cursor
                _buffer.Remove(cursorPosition - 1);
                return ClampCursorPosition(cursorPosition - 1);
            }

            _buffer.Insert(c, cursorPosition);
            return ClampCursorPosition(cursorPosition + 1);
        }

        public int MoveCursorToEndOfNextWord(int pos) {
            if (pos >= _buffer.Length) return _buffer.Length;

            if (char.IsLetter(_buffer[pos])) {
                // move to the end of this word
                while (pos < _buffer.Length && char.IsLetter(_buffer[pos])) {
                    pos++;
                }
                return pos;
            }

            // move to the start of the next word
            while (pos < _buffer.Length && !char.IsLetter(_buffer[pos])) {
                pos++;
            }

            return pos;
        }

        public int MoveCursorToEndOfPreviousWord(int pos) {
            if (pos <= 0) return 0;

            if (char.IsLetter(_buffer[pos - 1])) {
                // move to (1 before) the end of this word (going backwards)
                while (pos > 0 && char.IsLetter(_buffer[pos - 1])) {
                    pos--;
                }
                return pos;
            }

            // move to (1 before) the start of the next word (going backwards)
            while (pos > 0 && !char.IsLetter(_buffer[pos - 1])) {
                pos--;
            }

            return pos;
        }

        public int MoveCursorToLineStart(int pos) {
            int prevNewLine = _buffer.PrevIndexOf("\n", pos - 1);
            if (prevNewLine == -1) {
                return 0;
            }
            return prevNewLine + 1;
        }

        int max(int a, int b) {
            return a > b ? a : b;
        }

        public int MoveCursorUpALine(int pos) {
            pos = ClampCursorPosition(pos);

            int startOfThisLine = MoveCursorToLineStart(pos);
            if (startOfThisLine == 0) {
                return 0;
            }

            int startOfPreviousLine = MoveCursorToLineStart(startOfThisLine - 1);
            int endOfPreviousLine = startOfThisLine - 1;
            return min(endOfPreviousLine, startOfPreviousLine + (pos - startOfThisLine));
        }

        int min(int a, int b) {
            return a < b ? a : b;
        }

        public int MoveCursorDownALine(int pos) {
            // Using this algorithm: newPosition = min(endOfNextLine, startOfNextLine + (pos - startOfThisLine))

            pos = ClampCursorPosition(pos);
            int startOfThisLine = MoveCursorToLineStart(pos);

            int endOfThisLine = MoveCursorToEndOfLine(pos);
            if (endOfThisLine == _buffer.Length) {
                return endOfThisLine;
            }
            int startOfNextLine = endOfThisLine + 1;
            int endOfNextLine = MoveCursorToEndOfLine(startOfNextLine);

            return min(endOfNextLine, startOfNextLine + (pos - startOfThisLine));
        }

        public int MoveCursorToEndOfLine(int pos) {
            pos = _buffer.IndexOf("\n", pos);
            if (pos == -1) {
                pos = _buffer.Length;
            }
            return pos;
        }

        /// <summary>
        /// The behaviour when we press 'home' in a typical editor
        /// </summary>
        public int MoveCursorToHomeOfLine(int pos) {
            // if we are at the start of the line, dont move anywhere
            if (pos > 0 && _buffer[pos - 1] == '\n') {
                return pos;
            }

            // we don't actually want to go to the start of the line.
            // we want to go the first non-whitespace character in the line.
            // and if the cursor is already there or before there, _then_ we go to the start of the line.
            // Some editors will go back to the first non-whitespace char if we are at the very start of the line,
            // but I think that is kinda annoying so I won't implement that

            int lineStart = MoveCursorToLineStart(pos);
            if (lineStart == pos) {
                return pos;
            }

            int firstNonWhitspace = lineStart;
            while (
                firstNonWhitspace < _buffer.Length && 
                char.IsWhiteSpace(_buffer[firstNonWhitspace])
            ) {
                firstNonWhitspace++;
            }

            return firstNonWhitspace;
        }

        public int CountOccurrances(string s) {
            int count = 0;
            for(int i = 0; i < _buffer.Length; i++) {
                if (_buffer.CompareAtPosition(s, i)) {
                    count++;
                }
            }
            return count;
        }

#if DEBUG
        public static void RunTests(Testing t) {
            t.Run("MoveCursorToStartOfLine - tests", (ctx) => {
                // TODO: write tests
            });
        }
#endif
    }
}
