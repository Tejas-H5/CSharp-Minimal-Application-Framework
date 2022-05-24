using System;

namespace MinimalAF {
    // Why does this work
    public ref struct StringIterator {
        readonly ReadOnlySpan<char> _str;
        readonly string _delimiter;
        private readonly bool _skipEmpty;

        private int _index;
        private int _nextIndex;

        public int Count {
            get {
                if (_index >= _str.Length) {
                    return 0;
                }

                int lastIndex = _index;
                int lastNextIndex = _nextIndex;
                int count = 0;

                while (MoveNext()) {
                    count++;
                }

                _index = lastIndex;
                _nextIndex = lastNextIndex;

                return count;
            }
        }

        public StringIterator(ReadOnlySpan<char> str, string delimiter, bool skipEmpty = true) {
            if (delimiter == "") {
                throw new ArgumentException("cant have an empty delimiter.");
            }

            this._str = str;
            this._delimiter = delimiter;
            this._skipEmpty = skipEmpty;

            this._index = 0;
            _nextIndex = -delimiter.Length;
        }

        public StringIterator GetEnumerator() {
            return this;
        }

        public ReadOnlySpan<char> GetNext() {
            MoveNext();
            return Current;
        }


        private int FindNext() {
            int nextIndex = _str.Slice(_index)
                .IndexOf(_delimiter);

            if (nextIndex == -1) {
                return -1;
            }

            return nextIndex + _index;
        }

        public ReadOnlySpan<char> Current {
            get {
                return _str.Slice(_index, _nextIndex - _index);
            }
        }

        bool Step() {
            if (_nextIndex >= _str.Length) {
                return false;
            }

            _index = _nextIndex + _delimiter.Length;

            if (_index == _str.Length) {
                _nextIndex = _index;
            } else {
                _nextIndex = FindNext();
                if (_nextIndex == -1) {
                    _nextIndex = _str.Length;
                }
            }

            return true;
        }

        public bool MoveNext() {
            do {
                if (!Step()) {
                    return false;
                }
            } while (_skipEmpty && Current == "");

            return true;
        }

        public void Reset() {
            this._index = 0;
            _nextIndex = -_delimiter.Length;
        }
    }
}
