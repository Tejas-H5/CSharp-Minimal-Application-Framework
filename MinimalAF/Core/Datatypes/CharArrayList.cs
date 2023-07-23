using System;

namespace MinimalAF {
    /// <summary>
    /// C# strings actually allocate. So for most games, creating a lot of tiny strings every frame
    /// will put pressure on the GC. Or does it? 
    /// 
    /// I made this thing to test that theory. And if it works, I will recommend it to people
    /// </summary>

    // Naming conventions: https://cplusplus.com/reference/cstdio/printf/
    public struct CharArrayList {

        public static void Test() {
            CharArrayList _buffer = new CharArrayList { };
            _buffer.Clear();

            {
                float val = 1.0f;
                _buffer.Str(val.ToString()); _buffer.Str(":\n"); _buffer.Float(val);
            }

            {
                double val = 1.0 / 3.0;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Double(val);
                val = double.PositiveInfinity;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Double(val);
                val = double.NegativeInfinity;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Double(val);
                val = double.NaN;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Double(val);
            }

            {
                string val = "String test";
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Str(val);
            }
            {
                int val = 1;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Int(val);

                val = 2000;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Int(val);

                val = -1;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Int(val);

                val = -2000;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Int(val);
            }

            {
                var val = ulong.MaxValue;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.ULong(val);
            }

            {
                var val = long.MinValue;
                _buffer.Str("\n\n" + val.ToString()); _buffer.Str(":\n"); _buffer.Long(val);
            }

            Console.Write(((ReadOnlySpan<char>)_buffer).ToString());
            Console.WriteLine("== Done ==");
        }

        public int Length => Chars.Length;

        public ArrayList<char> Chars = new ArrayList<char> { };

        public char this[int index] {
            get => Chars.Data[index];
            set => Chars.Data[index] = value;
        }

        public int IndexOf(ReadOnlySpan<char> subsequence, int pos) {
            for (; pos < Length - subsequence.Length; pos++) {
                if (CompareSlice(subsequence, pos)) {
                    return pos;
                }
            }

            return -1;
        }

        public int PrevIndexOf(ReadOnlySpan<char> subsequence, int pos) {
            for (; pos >= 0; pos--) {
                if (CompareSlice(subsequence, pos)) {
                    return pos;
                }
            }

            return -1;
        }

        public bool CompareSlice(ReadOnlySpan<char> subsequence, int pos) {
            var span = Chars.AsSpan();
            var slice = span.Slice(pos, subsequence.Length);
            return slice.Equals(subsequence, StringComparison.Ordinal);
        }

        /// <summary>
        /// Mainly to prevent errors where we forget to clear this each frame
        /// </summary>
        public int MaxCapacity = 5096;

        public static implicit operator ReadOnlySpan<char>(CharArrayList ms) => ms.Chars;

        public CharArrayList() {}

        /// <summary>
        /// Gets the next code point in a C# UTF-16 string.
        /// 
        /// I yoinked it from this SO thread here: https://stackoverflow.com/questions/43564445/how-to-map-unicode-codepoints-from-an-utf-16-file-using-c
        /// </summary>
        public static (int, int) GetNextCodepoint(ReadOnlySpan<char> text, int pos) {
            char c1 = text[pos];
            if (char.IsHighSurrogate(c1)) {
                char c2 = text[pos + 1];
                int codePoint = ((c1 & 0x3ff) << 10) + (c2 & 0x3ff) + 0x10000;
                return (codePoint, 2);
            }

            return (c1, 1);
        }

        void AssertCapacityNotExceeded() {
            if (Chars.Length > MaxCapacity) {
                throw new Exception("You may have forgotten to call Clear() before appending to this text buffer. If that is not the case, set MaxCapacity to a higher number.");
            }
        }

        public (int, int) GetNextCodepoint(int pos) {
            return GetNextCodepoint(Chars, pos);
        }

        public void Clear() {
            Chars.Clear();
        }

        void appendString(string s) {
            for (var i = 0; i < s.Length; i++) {
                Chars.Append(s[i]);
            }
        }

        char getDigitChar(uint digit) {
            if (digit < 10) {
                return (char)('0' + digit);
            } 

            return (char)('A' + digit);
        }

        void appendULong(ulong integer, uint radix) {
            int bufferStart = Chars.Length;
            while (integer > 0) {
                var digit = integer % radix;
                char d = getDigitChar((uint)digit);

                integer /= radix;
                Chars.Append(d);
            }

            // the digits were put into the buffer backwards, so we need to reverse them
            var digitCount = Chars.Length - bufferStart;
            var n = Chars.Length - digitCount / 2;
            for (var i = bufferStart; i < n; i++) {
                var iFromBack = Chars.Length - 1 - (i - bufferStart);

                var temp = Chars.Data[iFromBack];
                Chars.Data[iFromBack] = Chars.Data[i];
                Chars.Data[i] = temp;
            }
        }


        public void Str(string s) {
            appendString(s);

            AssertCapacityNotExceeded();
        }

        public void Int(int integer, uint radix = 10) {
            Long((long)integer, radix);
        }

        public void Long(long integer, uint radix = 10) {
            if(integer < 0) {
                Chars.Append('-');
                integer = -integer;
            }

            appendULong((ulong)integer, radix);
            
            AssertCapacityNotExceeded();
        }

        public void UInt(uint integer, uint radix = 10) {
            ULong(integer, radix);
        }

        public void ULong(ulong integer, uint radix = 10) {
            appendULong(integer, radix);

            AssertCapacityNotExceeded();
        }

        public void Float(float f, uint maxPrecision = 7) {
            Double((double)f, maxPrecision);
        }


        public void Double(double f, uint maxPrecision = 15) {
            if (double.IsPositiveInfinity(f)) {
                Str("+Infinity");
                return;
            }

            if (double.IsNegativeInfinity(f)) {
                Str("-Infinity");
                return;
            }

            if (double.IsNaN(f)) {
                Str("NaN");
                return;
            }

            if (f < 0.0) {
                Chars.Append('-');
                f = -f;
            }

            int bufferStart = Chars.Length;
            var truncated = Math.Truncate(f);
            ulong intPart = (ulong)truncated;
            ULong(intPart, 10);

            int charsRendered = Chars.Length - bufferStart;
            if (charsRendered == 0) {
                Chars.Append('0');
            }
            maxPrecision -= (uint)(charsRendered);

            if (maxPrecision == 0) {
                return;
            }
                
            var decimalPart = f - truncated;
            var appendedADecimal = false;
            var currentDecimal = 0;
            var zeroesThatWerentAppended = 0;
            while (currentDecimal < maxPrecision) {
                currentDecimal++;

                decimalPart = (decimalPart * 10.0f) % 10.0f;
                char digit = getDigitChar((uint)Math.Truncate(decimalPart));
                if (digit == '0') {
                    zeroesThatWerentAppended++;
                    continue;
                }

                while(zeroesThatWerentAppended > 0) {
                    Chars.Append('0');
                    zeroesThatWerentAppended--;
                }

                if (!appendedADecimal) {
                    Str(".");
                    appendedADecimal = true;
                }
                Chars.Append(digit);
            }
        }

        static byte[] size1 = { 0 };
        static byte[] size2 = { 0, 0 };
        static byte[] size3 = { 0, 0, 0 };
        static byte[] size4 = { 0, 0, 0, 0 };

        /// <summary>
        /// Not threadsafe, since we are re-using static arrays to reduce allocations.
        /// </summary>
        public static byte[] CodePointToString(int codePoint) {
            // infer the size based on the data it contains
            int size;
            if ((codePoint >> 7) == 0) {
                size = 1;
            } else if ((codePoint >> 11) == 0) {
                size = 2;
            } else if ((codePoint >> 16) == 0) {
                size = 3;
            } else if ((codePoint >> 21) == 0) {
                size = 4;
            } else {
                throw new Exception("codepoint is too big");
            }

            if (size == 1) {
                size1[0] = (byte)codePoint;
                return size1;
            }

            if (size == 2) {
                size2[0] = (byte)(((codePoint >> 6) & 31) | (3 << 6));
                size2[1] = (byte)(((codePoint >> 0) & 63) | (1 << 7));
                return size2;
            }

            if (size == 3) {
                size3[0] = (byte)(((codePoint >> 12) & 15) | (7 << 5));
                size3[1] = (byte)(((codePoint >> 6) & 63) | (1 << 7));
                size3[2] = (byte)((codePoint & 63) | (1 << 7));
                return size3;
            }

            if (size == 4) {
                size4[0] = (byte)(((codePoint >> 18) & 7) | (15 << 4));
                size4[1] = (byte)(((codePoint >> 12) & 63) | (1 << 7));
                size4[2] = (byte)(((codePoint >> 6) & 63) | (1 << 7));
                size4[3] = (byte)((codePoint & 63) | (1 << 7));
                return size4;
            }

            throw new Exception("invalid size for code point");
        }
    }



    /*


    // Read the encoding table on this wikipedia article so that you can understand these comments: https://en.wikipedia.org/wiki/UTF-8.
    // Using SkiaSharp's  StringUtilities.GetUnicodeCharacterCode function is a pain -
    //  it doesn't work unless it is actually a unicode character, so I decided how about I just implemlent UTF-8 - How hard could it be?
    //  According to Wikipedia, it isn't very hard at all, and C# already natively has a way to encode
    //  a string as UTF-8 (not that I would need it though)

    // This class was mainly made because SkiaSharp's StringUtilities.GetUnicodeCharacterCode, but
    // I constantly have to allocate whenever I'm converting between a utf8 codepoint and a string.
    // So now I am just working with UTF-16 strings directly as I shuold have been.
    // Will probably delete this thing at some point, but I had fun writing it.
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
                //new TestCase{ String="\u0024", ExpectedSize=1, ExpectedCodePoint=0x24},
                //new TestCase{ String="A", ExpectedSize=1, ExpectedCodePoint=65},
                //new TestCase{ String="\u00A3", ExpectedSize=2, ExpectedCodePoint=0xA3},
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

                var backwards = Encoding.UTF8.GetString(CodePointToString(codePoint));
                Assert(backwards == testCase.String, $"Code point {expectedCodePointHex} - Backwards conversion - '{backwards}'");
            }
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
    */
}
