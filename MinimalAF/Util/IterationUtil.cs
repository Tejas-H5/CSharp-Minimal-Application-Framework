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
                int count = 1;

                while(!MoveNext()) {
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
            this.index = 0;
            this.delimiter = delimiter;
            this.skipEmpty = skipEmpty;

            nextIndex = -delimiter.Length;
            MoveNext();
        }

        public StringIterator GetEnumerator() {
            return this;
        }

        public ReadOnlySpan<char> GetNext() {
            var n = Current;
            MoveNext();
            return n;
        }


        private int FindNext() {
            int nextIndex = str.Slice(index + delimiter.Length)
                .IndexOf(delimiter);

            if(nextIndex == -1) {
                return -1;
            }

            return  nextIndex + index + delimiter.Length;
        }

        public ReadOnlySpan<char> Current {
            get {
                return str.Slice(index, nextIndex - index);
            }
        }

        bool Step() {
            index = nextIndex + delimiter.Length;

            if (index >= str.Length) {
                return true;
            }

            nextIndex = FindNext();
            if(nextIndex == -1) {
                nextIndex = str.Length;
            }

            return false;
        }

        public bool MoveNext() {
            do {
                if (Step()) {
                    return true;
                }
            } while (skipEmpty && Current == "");

            return false;
        }

        public void Reset() {
            index = 0;
        }
    }

    public static class IterationUtil {

        public static IndexedEnumerator<T> Enumerate<T>(this IList<T> enumerable) {
            return new IndexedEnumerator<T>(enumerable);
        }

        public struct IndexedEnumerator<T> {
            private readonly IList<T> list;
            private int index;


            public IndexedEnumerator<T> GetEnumerator() {
                return this;
            }

            public IndexedEnumerator(IList<T> list) {
                this.list = list;
                index = 0;
            }

            public (int, T) Current {
                get {
                    return (index, list[index]);
                }
            }

            public bool MoveNext() {
                index++;
                return list.Count >= index;
            }

            public void Reset() {
                index = 0;
            }
        }


        public static StringIterator IterSplit(this string str, string delimiter, bool skipEmpty = true, int startindex = 0) {
            return new StringIterator(str.AsSpan().Slice(startindex), delimiter, skipEmpty);
        }

        public static StringIterator IterSplit(this ReadOnlySpan<char> str, string delimiter, bool skipEmpty = true, int startindex = 0) {
            return new StringIterator(str.Slice(startindex), delimiter, skipEmpty);
        }


    }
}
