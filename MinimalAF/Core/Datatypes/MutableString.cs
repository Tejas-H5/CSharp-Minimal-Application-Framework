using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAF {
    // Read the encoding table on this wikipedia article so that you can understand these comments: https://en.wikipedia.org/wiki/UTF-8.
    // Using SkiaSharp's  StringUtilities.GetUnicodeCharacterCode function is a pain -
    //  it doesn't work unless it is actually a unicode character, so I decided how about I just implemlent UTF-8 - How hard could it be?
    //  According to Wikipedia, it isn't very hard at all, and C# already natively has a way to encode
    //  a string as UTF-8 (not that I would need it though)
    public struct MutableUT8String {
        public byte[] Bytes;    // This is just a buffer that can be overwritten or resized. 
        public int Length;      // This is the true length of this datastructure (in bytes, and not code points. sorry)

        public MutableUT8String(string text) {
            Bytes = Encoding.UTF8.GetBytes(text);
            Length = Bytes.Length;
        }

        struct TestCase {
            public string String;
            public int ExpectedCodePoint;
            public int ExpectedSize;
        }

        // TODO: remove these tests
        public static void Test() {
            void Assert(bool fact, string testName) {
                if (!fact) {
                    Console.WriteLine("[FAILED] - " + testName);
                } else {
                    Console.WriteLine("[PASSED] - " + testName);
                }
            }

            var testCases = new TestCase[] {
                new TestCase{ String="\u0024", ExpectedSize=1, ExpectedCodePoint=0x24},
                new TestCase{ String="\u00A3", ExpectedSize=2, ExpectedCodePoint=0xA3},
                new TestCase{ String="\u0418", ExpectedSize=2, ExpectedCodePoint=0x418},
                new TestCase{ String="\u0939", ExpectedSize=3, ExpectedCodePoint=0x939},
                new TestCase{ String="\u20AC", ExpectedSize=3, ExpectedCodePoint=0x20AC},
                new TestCase{ String="\uD55C", ExpectedSize=3, ExpectedCodePoint=0xD55C},
                new TestCase{ String="\U0002825F", ExpectedSize=4, ExpectedCodePoint=0x2825F},
            };

            for(var i = 0; i < testCases.Length; i++) {
                var testCase = testCases[i];
                var str = new MutableUT8String(testCase.String);
                int pos = 0, size, codePoint;
                (size, codePoint) = str.GetNextCodePoint(pos);
                var expectedCodePointHex = testCase.ExpectedCodePoint.ToString("X");
                Assert(size == testCase.ExpectedSize, $"Code point {expectedCodePointHex} - {size}");
                Assert(codePoint == testCase.ExpectedCodePoint, $"Code point {expectedCodePointHex} - {codePoint.ToString("X")}");

                var backwards = CodePointToString(codePoint, size);
                Assert(backwards == testCase.String, $"Code point {expectedCodePointHex} - Backwards conversion - '{backwards}'");
            }
        }

        public static string CodePointToString(int codePoint, int size) {
            if (size == 1) {
                var bytes = new byte[] { (byte)codePoint };
                return Encoding.UTF8.GetString(bytes);
            }

            if (size == 2) {
                var bytes = new byte[] { 
                    (byte)(((codePoint >> 6) & 31) + (3 << 6)),
                    (byte)(((codePoint >> 0) & 63) + (1 << 7))
                };
                return Encoding.UTF8.GetString(bytes);
            }

            if (size == 3) {
                var bytes = new byte[] {
                    (byte)(((codePoint >> 12) & 15) + (7 << 5)),
                    (byte)(((codePoint >> 6) & 63) + (1 << 7)),
                    (byte)((codePoint & 63) + (1 << 7))
                };
                return Encoding.UTF8.GetString(bytes);
            }

            if (size == 4) {
                var bytes = new byte[] {
                    (byte)(((codePoint >> 18) & 7) + (15 << 4)),
                    (byte)(((codePoint >> 12) & 63) + (1 << 7)),
                    (byte)(((codePoint >> 6) & 63) + (1 << 7)),
                    (byte)((codePoint & 63) + (1 << 7))
                };
                return Encoding.UTF8.GetString(bytes);
            }

            throw new Exception("invalid size for code point");
        }

        bool Is10XXXXXX(byte b1) {
            return (
                // (b1 & (00000011 << 6)) == 10000000
                (b1 & (0x3 << 6)) == (0x1 << 7)
            );
        }

        int Get__XXXXXX(byte b1) {
            return b1 & 63;
        }

        // return size, codePoint
        public (int, int) GetNextCodePoint(int pos) {
            var b1 = Bytes[pos];
            // (b1 & 1xxxxxxx) == 0
            if ((b1 & (1 << 7)) == 0x0) {
                return (1, b1);
            }

            var b2 = Bytes[pos + 1];
            // 110xxxxx 10xxxxxx
            if (
                (b1 & (7 << 5)) == (3 << 6) // ((b1 & (00000111) << 5 )) == (00000011 << 6)
                && Is10XXXXXX(b2)
            ) {
                return (
                    2, 
                    ((b1 & ((byte)31)) << 6) + Get__XXXXXX(b2)
                );
            }

            var b3 = Bytes[pos + 2];
            // 1110xxxx 10xxxxxx 10xxxxxx 
            if (
                (b1 & (15 << 4)) == (7 << 5)
                && Is10XXXXXX(b2)
                && Is10XXXXXX(b3)
            ) {
                return (
                    3,
                    ((b1 & ((byte)15)) << 12) + (Get__XXXXXX(b2) << 6) + (Get__XXXXXX(b3))
                );
            }

            var b4 = Bytes[pos + 3];
            // 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
            if (
                (b1 & (31 << 3)) == (15 << 4)
                && Is10XXXXXX(b2)
                && Is10XXXXXX(b3)
                && Is10XXXXXX(b4)
            ) {
                return (
                    4,
                    ((b1 & ((byte)15)) << 18) + (Get__XXXXXX(b2) << 12) + (Get__XXXXXX(b3) << 6) + (Get__XXXXXX(b4))
                );
            }

            throw new Exception("Not the start of a unicode char. I need to handle this edge case properly though");
        }
    }


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

        public void RemoveRange(int startIn, int endIn) {
            // TODO: implement this with an Insert method when I make it

            int start = MathHelpers.Min(startIn, endIn);
            int end = MathHelpers.Max(startIn, endIn);

            int amount = end - start;
            if (amount == 0) {
                return;
            }

            for (int i = start; i + amount < Length; i++) {
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

        public int IndexOf(string s, int start) {
            if (start < 0) {
                start = 0;
            }

            for (int pos = start; pos < length; pos++) {
                if (CompareAtPosition(s, pos)) {
                    return pos;
                }
            }

            return -1;
        }

        public int PrevIndexOf(string s, int start) {
            if (start < 0) {
                start = 0;
            }

            for (int pos = start; pos >= 0; pos--) {
                if (CompareAtPosition(s, pos)) {
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
                    if (CompareAtPosition(s, pos)) {
                        return pos;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Use this for debug purposes or one-time things only, it allocates a whole array
        /// each time. 
        /// 
        /// An example use case is for a modal that wants a user to input an int value, and
        /// they input that stuff into this. You would only call ToString()
        /// if you want to pass this input to the int.TryParse or whatever external parsing function
        /// that usually takes a string when a user presses "Enter" or however your modal works
        /// </summary>
        public string BuildString() {
            return string.Create(length, characters, (chars, state) => {
                for(int i = 0; i < length; i++) {
                    chars[i] = state[i];
                }
            });
        }
    }
}
