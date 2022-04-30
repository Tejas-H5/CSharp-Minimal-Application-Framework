using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF {

    public ref struct StringIterator {
        readonly ReadOnlySpan<char> str;
        readonly string delimiter;
        private readonly bool skipEmpty;

        private int index;
        private int nextIndex;

        public int Count {
            get {
                if(index >= str.Length) {
                    return 0;
                }

                int lastIndex = index;
                int lastNextIndex = nextIndex;
                int count = 0;

                while(MoveNext()) {
                    count++;
                }

                index = lastIndex;
                nextIndex = lastNextIndex;

                return count;
            }
        }

        public StringIterator(ReadOnlySpan<char> str, string delimiter, bool skipEmpty = true) {
            if(delimiter == "") {
                throw new ArgumentException("cant have an empty delimiter.");
            }

            this.str = str;
            this.delimiter = delimiter;
            this.skipEmpty = skipEmpty;

            this.index = 0;
            nextIndex = -delimiter.Length;
        }

        public StringIterator GetEnumerator() {
            return this;
        }

        public ReadOnlySpan<char> GetNext() {
            MoveNext();
            return Current;
        }


        private int FindNext() {
            int nextIndex = str.Slice(index)
                .IndexOf(delimiter);

            if(nextIndex == -1) {
                return -1;
            }

            return  nextIndex + index;
        }

        public ReadOnlySpan<char> Current {
            get {
                return str.Slice(index, nextIndex - index);
            }
        }

        bool Step() {
            if (nextIndex >= str.Length) {
                return false;
            }

            index = nextIndex + delimiter.Length;

            if(index == str.Length) {
                nextIndex = index;
            } else {
                nextIndex = FindNext();
                if(nextIndex == -1) {
                    nextIndex = str.Length;
                }
            }

            return true;
        }

        public bool MoveNext() {
            do {
                if (!Step()) {
                    return false;
                }
            } while (skipEmpty && Current == "");

            return true;
        }

        public void Reset() {
            this.index = 0;
            nextIndex = -delimiter.Length;
        }
    }
}
