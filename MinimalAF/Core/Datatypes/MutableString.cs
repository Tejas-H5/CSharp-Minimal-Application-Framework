using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAF {
    /// <summary>
    /// To reduce string allocations when printing out text in a render loop
    /// </summary>
    public class MutableString {
        char[] characters;
        int length = 0;
        public char[] Characters => characters;
        public int Length => length;

        public char this[int i] {
            get {
                return characters[i];
            }
            set {
                if (value < 0) throw new IndexOutOfRangeException();
                if (value >= length) throw new IndexOutOfRangeException();

                characters[i] = value;
            }
        }

        public MutableString() : this(0) { }

        public MutableString(int len) {
            characters = new char[len];
        }

        public void Clear() {
            length = 0;
        }

        void ResizeBackingBuffer(int newLen) {
            var newCharacters = new char[newLen];
            Array.Copy(characters, newCharacters, characters.Length);
            characters = newCharacters;
        }

        public void Resize(int newSize) {
            ResizeBackingBuffer(newSize);
            length = newSize;
        }


        public void RemoveRange(int start, int end) {
            // TODO: implement this with an Insert method when I make it

            int amount = end - start;
            for (int i = start; i < Length - amount; i++) {
                this.characters[i] = this.characters[i + amount];
            }

            length -= amount;
        }

        void ResizeBackingBufferIfRequired(int additionalChars) {
            int newLength = length + additionalChars;
            if (newLength > 100000) {
                throw new Exception("WTF BRO");
            }

            // TODO: test if this is any good of a resizing algorithm or not
            if (newLength > this.characters.Length) {
                int newSize = this.characters.Length > 0 ? this.characters.Length : 1;
                while (newSize < newLength) {
                    newSize *= 2;
                }
                ResizeBackingBuffer(newSize);
            }

            length = newLength;
        }

        public void Insert(char c, int pos) {
            if (pos < 0 || pos > length) return;
            ResizeBackingBufferIfRequired(1);

            // moves all the characters to the right of pos one more to the right
            // it is supposed to be fast because of cache coherency? I heard that on a youtube video somewhere but its hard to believe
            // TODO: test somehow
            for (var i = length - 1; i > pos; i--) {
                characters[i] = characters[i - 1];
            }
            characters[pos] = c;
        }

        public void Remove(int pos) {
            if (pos < 0 || pos >= length) return;

            // moves all the characters to the right of pos one to the left
            for (var i = pos; i + 1 < length; i++) {
                characters[i] = characters[i + 1];
            }
            length--;
        }

        public bool Contanis(char c) {
            return Array.IndexOf(characters, c) != -1;
        }

        public bool CompareAtPosition(string s, int pos) {
            if (pos + s.Length > length) return false;

            for (int i = 0; i < s.Length; i++) {
                if (characters[pos + i] != s[i]) {
                    return false;
                }
            }

            return true;
        }

        public bool ReverseCompareAtPosition(string s, int pos) {
            if (pos - (s.Length - 1) < 0) return false;

            for (int i = 0; i < s.Length; i++) {
                if (characters[pos - i] != s[s.Length - 1 - i]) {
                    return false;
                }
            }

            return true;
        }

        public int IndexOf(string s, int start) {
            for(int pos = start; pos < length; pos++) {
                if (CompareAtPosition(s, pos)) {
                    return pos;
                }
            }

            return -1;
        }

        public int PrevIndexOf(string s, int start) {
            for (int pos = start; pos >= 0; pos--) {
                if (ReverseCompareAtPosition(s, pos)) {
                    return pos;
                }
            }

            return -1;
        }

        public int IndexOfAny(string[] strings, int start) {
            for (int pos = start; pos < length; pos++) {
                for (int i = 0; i < strings.Length; i++) {
                    string s = strings[i];
                    if (CompareAtPosition(s, pos)) {
                        return pos;
                    }
                }
            }

            return -1;
        }

        public int PrevIndexOfAny(string[] strings, int start) {
            for (int pos = start; pos >= 0; pos--) {
                for (int i = 0; i < strings.Length; i++) {
                    string s = strings[i];
                    if (ReverseCompareAtPosition(s, pos)) {
                        return pos;
                    }
                }
            }

            return -1;
        }
    }
}
