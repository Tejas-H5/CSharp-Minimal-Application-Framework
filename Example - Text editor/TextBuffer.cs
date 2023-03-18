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

        public int MoveCursor(int newPosition) {
            if (newPosition < 0) newPosition = 0;
            if (newPosition > _buffer.Length) newPosition = _buffer.Length;
            return newPosition;
        }

        public int AddLetterAtCursor(int cursorPosition, char c) {
            if (c == '\r') return cursorPosition;

            if (c == '\b') {
                // we really want to remove a character that is behind the cursor
                _buffer.Remove(cursorPosition - 1);
                return MoveCursor(cursorPosition - 1);
            }

            _buffer.Insert(c, cursorPosition);
            return MoveCursor(cursorPosition + 1);
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

        public int MoveCursorUpALine(int pos) {
            int prevNewLine  = _buffer.PrevIndexOf("\n", pos);
            if (prevNewLine == -1) {
                // We are at the start of the file, don't move the cursor anywhere ? 
                return pos;
            }

            int currentDistFromPrevNewLine = pos - prevNewLine;
            int previousPreviousNewline = _buffer.PrevIndexOf("\n", pos - currentDistFromPrevNewLine - 1);

            if (previousPreviousNewline == -1) {
                // we found prevNewLine, so we are probably 1 line from the top of the file.
                previousPreviousNewline = 0;
            }

            int previousLineLength = prevNewLine - previousPreviousNewline;
            if (currentDistFromPrevNewLine > previousLineLength) {
                return prevNewLine;
            }

            return previousPreviousNewline + currentDistFromPrevNewLine;
        }

        public int MoveCursorDownALine(int pos) {
            int prevNewLine = _buffer.PrevIndexOf("\n", pos - 1);
            if (prevNewLine == -1) {
                // We are at the start of the file, can just use 0
                prevNewLine = 0;
            }
            int currentDistFromPrevNewLine = pos - prevNewLine;

            int nextNewLine = _buffer.IndexOf("\n", pos);
            if (nextNewLine == -1) {
                // we are at the end of the file, dont move anywhere
                return pos;
            }

            int nextNextNewLine = _buffer.IndexOf("\n", nextNewLine + 1);
            if (nextNextNewLine == -1) {
                // we found nextNewLine so we are 1 line from the end of the file.
                nextNextNewLine = _buffer.Length;
            }

            int nextLineLength = nextNextNewLine - nextNewLine;
            if (currentDistFromPrevNewLine > nextLineLength) {
                return nextNextNewLine;
            }

            return nextNewLine + currentDistFromPrevNewLine;
        }

        /// <summary>
        /// You would use this to iterate an immediate mode render loop
        /// </summary>
        public int MoveCursorToNextLine(int pos) {
            return _buffer.IndexOf("\n", pos + 1);
        }

    }
}
