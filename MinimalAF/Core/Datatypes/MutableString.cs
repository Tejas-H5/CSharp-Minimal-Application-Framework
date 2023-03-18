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
        }

        public MutableString() : this(0) { }

        public MutableString(int len) {
            characters = new char[len];
        }

        public void Clear() {
            length = 0;
        }

        public void Resize(int newLen) {
            var newCharacters = new char[newLen];
            Array.Copy(characters, newCharacters, characters.Length);
            characters = newCharacters;
        }

        void ResizeIfRequired(int minSize) {
            // TODO: test if this is any good of a resizing algorithm or not
            if (minSize > this.characters.Length) {
                int newSize = this.characters.Length > 0 ? this.characters.Length : 1;
                while (newSize < minSize) {
                    newSize *= 2;
                }
                Resize(newSize);
            }
        }

        public void Append(ReadOnlySpan<char> characters) {
            ResizeIfRequired(length + characters.Length);

            for (var i = 0; i < characters.Length; i++) {
                this.characters[length + i] = characters[i];
            }

            length += characters.Length;
        }

        public void Append(char c) {
            ResizeIfRequired(length + 1);

            this.characters[length] = c;
            length++;
        }

        public void Insert(char c, int pos) {
            if (pos < 0 || pos > length) return;
            ResizeIfRequired(length + 1);

            // moves all the characters to the right of pos one more to the right
            // it is supposed to be fast because of cache coherency? I heard that on a youtube video somewhere but its hard to believe
            // TODO: test somehow
            for (var i = length; i > pos; i--) {
                characters[i] = characters[i - 1];
            }
            characters[pos] = c;
            length++;
        }

        public void Remove(int pos) {
            if (pos < 0 || pos >= length) return;

            // moves all the characters to the right of pos one to the left
            for (var i = pos; i < length; i++) {
                characters[i] = characters[i + 1];
            }
            length--;
        }

        public bool Contanis(char c) {
            return Array.IndexOf(characters, c) != -1;
        }
    }
}
